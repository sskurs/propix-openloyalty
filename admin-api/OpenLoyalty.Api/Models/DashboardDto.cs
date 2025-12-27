using System;
using System.Collections.Generic;

namespace OpenLoyalty.Api.Models
{
    public class DashboardDto
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int NewMembersLastMonth { get; set; }
        public decimal TotalPointsIssued { get; set; }
        public decimal TotalPointsRedeemed { get; set; }
        public decimal PointsBreakage { get; set; } // Points expired or lost
        public List<ActivityLog> RecentActivities { get; set; } = new();
        
        // Simple data for charts
        public List<PointsChartPoint> PointsHistory { get; set; } = new();
        public List<DistributionPoint> TierDistribution { get; set; } = new();
        public List<CampaignPerformancePoint> CampaignPerformance { get; set; } = new();
    }

    public class PointsChartPoint
    {
        public string Label { get; set; } = string.Empty; // e.g. "Mon", "Tue" or date
        public decimal Issued { get; set; }
        public decimal Redeemed { get; set; }
    }

    public class DistributionPoint
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CampaignPerformancePoint
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
