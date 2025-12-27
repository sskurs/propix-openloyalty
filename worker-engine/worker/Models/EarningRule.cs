using System;

namespace Worker.Models
{
    public class EarningRule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string? ConditionJson { get; set; }
        public string? PointsJson { get; set; }
        public string? LimitsJson { get; set; }
        public string? Expression { get; set; }
        public string? Status { get; set; } = "active";
        public bool Active { get; set; }
        public bool IsActive { get; set; } = true;
        public int Version { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = "system";

    }
}
