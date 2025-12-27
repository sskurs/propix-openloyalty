using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Customer.Api.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenLoyalty.Customer.Api.Controllers
{
    [ApiController]
    [Route("api/rewards")]
    public class RewardsController : ControllerBase
    {
        private readonly LoyaltyDbContext _db;
        private readonly ILogger<RewardsController> _logger;

        public RewardsController(LoyaltyDbContext db, ILogger<RewardsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Get current points balance for the authenticated user
        /// </summary>
        [HttpGet("me/points")]
        public async Task<IActionResult> GetMyPoints()
        {
            // TODO: Get userId from JWT token when authentication is implemented
            // For now, use a query parameter for testing
            var userId = Request.Query["userId"].ToString();
            
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "userId query parameter is required for testing" });
            }

            _logger.LogInformation("Getting points balance for user {UserId}", userId);

            var memberPoints = await _db.MemberPoints
                .FirstOrDefaultAsync(m => m.MemberId == userId);

            if (memberPoints == null)
            {
                // Return default values for new members
                return Ok(new
                {
                    pointsBalance = 0,
                    lifetimePoints = 0,
                    tier = "BRONZE",
                    pointsEarnedThisMonth = 0,
                    pointsRedeemed = 0,
                    lastUpdated = (DateTime?)null
                });
            }

            return Ok(new
            {
                pointsBalance = memberPoints.PointsBalance,
                lifetimePoints = memberPoints.LifetimePoints,
                tier = memberPoints.Tier,
                pointsEarnedThisMonth = memberPoints.PointsEarnedThisMonth,
                pointsRedeemed = memberPoints.PointsRedeemed,
                lastUpdated = memberPoints.LastUpdatedAt
            });
        }

        /// <summary>
        /// Get points transaction history for the authenticated user
        /// </summary>
        [HttpGet("me/points/history")]
        public async Task<IActionResult> GetPointsHistory(
            [FromQuery] string? userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "userId query parameter is required for testing" });
            }

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            _logger.LogInformation("Getting points history for user {UserId}, page {Page}", userId, page);

            var query = _db.PointsTransactions
                .Where(pt => pt.MemberId == userId)
                .OrderByDescending(pt => pt.CreatedAt);

            var total = await query.CountAsync();

            var transactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(pt => new
                {
                    id = pt.Id,
                    type = pt.TransactionType,
                    points = pt.Points,
                    balanceAfter = pt.BalanceAfter,
                    description = pt.Description,
                    campaignId = pt.CampaignId,
                    orderId = pt.OrderId,
                    createdAt = pt.CreatedAt
                })
                .ToListAsync();

            return Ok(new
            {
                transactions,
                pagination = new
                {
                    page,
                    pageSize,
                    total,
                    totalPages = (int)Math.Ceiling(total / (double)pageSize)
                }
            });
        }

        /// <summary>
        /// Get all campaign rewards earned by the authenticated user
        /// </summary>
        [HttpGet("me/campaigns/rewards")]
        public async Task<IActionResult> GetMyCampaignRewards([FromQuery] string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "userId query parameter is required for testing" });
            }

            _logger.LogInformation("Getting campaign rewards for user {UserId}", userId);

            // Get all campaign executions for this user
            var rewards = await _db.CampaignExecutions
                .Where(ce => ce.MemberId == userId)
                .Join(_db.Campaigns,
                    ce => ce.CampaignId,
                    c => c.Id,
                    (ce, c) => new
                    {
                        campaignId = c.Id.ToString(),
                        campaignName = c.Name,
                        campaignCode = c.Code,
                        rewardType = ce.RewardType,
                        rewardDetails = ce.ExecutionResultJson,
                        transactionId = ce.TransactionId,
                        executedAt = ce.ExecutedAt
                    })
                .OrderByDescending(r => r.executedAt)
                .Take(100) // Limit to last 100 rewards
                .ToListAsync();

            return Ok(new { rewards, count = rewards.Count });
        }

        /// <summary>
        /// Get available campaigns (active campaigns)
        /// </summary>
        [HttpGet("campaigns/available")]
        public async Task<IActionResult> GetAvailableCampaigns([FromQuery] string? userId)
        {
            _logger.LogInformation("Getting available campaigns");

            var now = DateTime.UtcNow;

            // Get active campaigns
            var campaigns = await _db.Campaigns
                .Where(c => c.Status == "ACTIVE")
                .Where(c => c.StartAt <= now && (!c.EndAt.HasValue || c.EndAt.Value >= now))
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    code = c.Code,
                    description = c.Description,
                    startAt = c.StartAt,
                    endAt = c.EndAt,
                    status = c.Status
                })
                .ToListAsync();

            // If userId provided, check enrollment status
            if (!string.IsNullOrEmpty(userId))
            {
                var enrolledCampaignIds = await _db.CampaignEnrollments
                    .Where(e => e.MemberId == userId && e.Status == "ACTIVE")
                    .Select(e => e.CampaignId)
                    .ToListAsync();

                var result = campaigns.Select(c => new
                {
                    c.id,
                    c.name,
                    c.code,
                    c.description,
                    c.startAt,
                    c.endAt,
                    c.status,
                    isEnrolled = enrolledCampaignIds.Contains(c.id.ToString())
                });

                return Ok(new { campaigns = result, count = campaigns.Count });
            }

            return Ok(new { campaigns, count = campaigns.Count });
        }

        /// <summary>
        /// Enroll in a campaign
        /// </summary>
        [HttpPost("campaigns/{id}/enroll")]
        public async Task<IActionResult> EnrollInCampaign(string id, [FromQuery] string? userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "userId query parameter is required for testing" });
            }

            _logger.LogInformation("User {UserId} enrolling in campaign {CampaignId}", userId, id);

            // Parse campaign ID
            if (!Guid.TryParse(id, out var campaignGuid))
            {
                return BadRequest(new { error = "Invalid campaign ID format" });
            }

            // Check if campaign exists and is active
            var campaign = await _db.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignGuid);
            if (campaign == null || campaign.Status != "ACTIVE")
            {
                return NotFound(new { error = "Campaign not found or inactive" });
            }

            // Check if already enrolled
            var existing = await _db.CampaignEnrollments
                .FirstOrDefaultAsync(e => e.CampaignId == id && e.MemberId == userId);

            if (existing != null)
            {
                return BadRequest(new { error = "Already enrolled in this campaign" });
            }

            // Create enrollment
            var enrollment = new OpenLoyalty.Customer.Api.Models.CampaignEnrollment
            {
                CampaignId = id,
                MemberId = userId,
                EnrolledAt = DateTime.UtcNow,
                Status = "ACTIVE"
            };

            _db.CampaignEnrollments.Add(enrollment);
            await _db.SaveChangesAsync();

            _logger.LogInformation("User {UserId} successfully enrolled in campaign {CampaignId}", userId, id);

            return Ok(new
            {
                message = "Successfully enrolled in campaign",
                enrollment = new
                {
                    id = enrollment.Id,
                    campaignId = enrollment.CampaignId,
                    status = enrollment.Status,
                    enrolledAt = enrollment.EnrolledAt
                }
            });
        }
    }
}
