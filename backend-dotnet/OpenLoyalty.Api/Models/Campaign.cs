using System;

namespace OpenLoyalty.Api.Models
{
    public class Campaign
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "draft";
        public string? EventType { get; set; }
        public string? TargetSegment { get; set; } // JSON
        public string ConditionRules { get; set; } // JSON
        public string RewardRules { get; set; } // JSON
        public string? Limits { get; set; } // JSON
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
