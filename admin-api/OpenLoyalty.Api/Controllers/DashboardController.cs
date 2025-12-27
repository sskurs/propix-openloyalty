using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLoyalty.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(LoyaltyDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboardStats()
        {
            try
            {
                var now = DateTime.UtcNow;
                var lastMonth = now.AddDays(-30);
                var last7Days = now.AddDays(-7);

                // Member statistics
                var totalMembers = await _context.Members.CountAsync();
                var activeMembers = await _context.Members.CountAsync(m => m.Status == "active");
                var newMembers = await _context.Members.CountAsync(m => m.JoinDate >= lastMonth);

                // Points statistics from member_points table
                decimal totalPointsIssued = 0;
                decimal totalPointsRedeemed = 0;

                try
                {
                    // Try to get points from member_points table
                    var pointsQuery = await _context.Database
                        .SqlQueryRaw<PointsStats>("SELECT COALESCE(SUM(lifetime_points), 0) as TotalIssued, COALESCE(SUM(points_redeemed), 0) as TotalRedeemed FROM member_points")
                        .ToListAsync();
                    
                    if (pointsQuery.Any())
                    {
                        totalPointsIssued = pointsQuery.First().TotalIssued;
                        totalPointsRedeemed = pointsQuery.First().TotalRedeemed;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch from member_points table, using fallback");
                    // Fallback to WalletLogs if member_points doesn't exist yet
                    totalPointsIssued = await _context.WalletLogs
                        .Where(l => l.WalletType == "points" && l.Direction == "IN")
                        .SumAsync(l => (decimal?)l.Amount) ?? 0;

                    totalPointsRedeemed = await _context.WalletLogs
                        .Where(l => l.WalletType == "points" && l.Direction == "OUT")
                        .SumAsync(l => (decimal?)l.Amount) ?? 0;
                }

                // Recent activities from campaign executions
                var recentActivities = new List<ActivityLog>();
                try
                {
                    var executions = await _context.Database
                        .SqlQueryRaw<CampaignExecutionActivity>(@"
                            SELECT 
                                ce.id::text as Id,
                                'Campaign Reward' as Type,
                                CONCAT('Member earned reward from campaign') as Description,
                                ce.executed_at as CreatedAt
                            FROM campaign_executions ce
                            ORDER BY ce.executed_at DESC
                            LIMIT 10")
                        .ToListAsync();

                    recentActivities = executions.Select(e => new ActivityLog
                    {
                        Id = Guid.TryParse(e.Id, out var guid) ? guid : Guid.NewGuid(),
                        Type = e.Type,
                        Description = e.Description,
                        CreatedAt = e.CreatedAt
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch campaign executions");
                }

                // Tier distribution from member_points
                var tierDistribution = new List<DistributionPoint>();
                try
                {
                    var tiers = await _context.Database
                        .SqlQueryRaw<TierCount>("SELECT tier as Name, COUNT(*)::int as Count FROM member_points GROUP BY tier")
                        .ToListAsync();

                    tierDistribution = tiers.Select(t => new DistributionPoint
                    {
                        Name = t.Name ?? "BRONZE",
                        Count = t.Count
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch tier distribution, using member tiers");
                    // Fallback to member tiers
                    tierDistribution = await _context.Members
                        .GroupBy(m => m.Tier.Name)
                        .Select(g => new DistributionPoint { Name = g.Key ?? "None", Count = g.Count() })
                        .ToListAsync();
                }

                // Points History (Last 7 days) from points_transactions
                var pointsHistory = new List<PointsChartPoint>();
                try
                {
                    var history = await _context.Database
                        .SqlQueryRaw<DailyPoints>(@"
                            SELECT 
                                DATE(created_at) as Date,
                                SUM(CASE WHEN transaction_type = 'EARNED' THEN points ELSE 0 END)::decimal as Issued,
                                SUM(CASE WHEN transaction_type = 'REDEEMED' THEN points ELSE 0 END)::decimal as Redeemed
                            FROM points_transactions
                            WHERE created_at >= {0}
                            GROUP BY DATE(created_at)
                            ORDER BY DATE(created_at)", last7Days)
                        .ToListAsync();

                    // Fill in missing days
                    pointsHistory = Enumerable.Range(0, 7).Select(i =>
                    {
                        var date = now.AddDays(-6 + i).Date;
                        var dayData = history.FirstOrDefault(h => h.Date.Date == date);
                        return new PointsChartPoint
                        {
                            Label = date.ToString("MMM dd"),
                            Issued = dayData?.Issued ?? 0,
                            Redeemed = dayData?.Redeemed ?? 0
                        };
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch points history, using fallback");
                    // Fallback to WalletLogs
                    var recentLogs = await _context.WalletLogs
                        .Where(l => l.WalletType == "points" && l.CreatedAt >= last7Days)
                        .Select(l => new { l.CreatedAt, l.Direction, l.Amount })
                        .ToListAsync();

                    pointsHistory = Enumerable.Range(0, 7).Select(i =>
                    {
                        var date = now.AddDays(-6 + i).Date;
                        var dailyLogs = recentLogs.Where(l => l.CreatedAt.Date == date);
                        return new PointsChartPoint
                        {
                            Label = date.ToString("MMM dd"),
                            Issued = dailyLogs.Where(l => l.Direction == "IN").Sum(l => l.Amount),
                            Redeemed = dailyLogs.Where(l => l.Direction == "OUT").Sum(l => l.Amount)
                        };
                    }).ToList();
                }

                // Campaign Performance from campaign_executions
                var performanceChart = new List<CampaignPerformancePoint>();
                try
                {
                    var performance = await _context.Database
                        .SqlQueryRaw<CampaignPerf>(@"
                            SELECT 
                                c.name as Name,
                                COUNT(ce.id)::int as Count
                            FROM campaigns c
                            LEFT JOIN campaign_executions ce ON c.id::text = ce.campaign_id::text
                            WHERE c.status = 'ACTIVE'
                            GROUP BY c.id, c.name
                            ORDER BY COUNT(ce.id) DESC
                            LIMIT 5")
                        .ToListAsync();

                    performanceChart = performance.Select(p => new CampaignPerformancePoint
                    {
                        Name = p.Name,
                        Count = p.Count
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not fetch campaign performance");
                }

                return Ok(new DashboardDto
                {
                    TotalMembers = totalMembers,
                    ActiveMembers = activeMembers,
                    NewMembersLastMonth = newMembers,
                    TotalPointsIssued = totalPointsIssued,
                    TotalPointsRedeemed = totalPointsRedeemed,
                    PointsBreakage = totalPointsIssued - totalPointsRedeemed,
                    RecentActivities = recentActivities,
                    TierDistribution = tierDistribution,
                    PointsHistory = pointsHistory,
                    CampaignPerformance = performanceChart
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard data");
                return StatusCode(500, new { error = "Failed to fetch dashboard data", details = ex.Message });
            }
        }

        // Helper classes for raw SQL queries
        public class PointsStats
        {
            public decimal TotalIssued { get; set; }
            public decimal TotalRedeemed { get; set; }
        }

        public class CampaignExecutionActivity
        {
            public string Id { get; set; } = "";
            public string Type { get; set; } = "";
            public string Description { get; set; } = "";
            public DateTime CreatedAt { get; set; }
        }

        public class TierCount
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
        }

        public class DailyPoints
        {
            public DateTime Date { get; set; }
            public decimal Issued { get; set; }
            public decimal Redeemed { get; set; }
        }

        public class CampaignPerf
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
        }
    }
}
