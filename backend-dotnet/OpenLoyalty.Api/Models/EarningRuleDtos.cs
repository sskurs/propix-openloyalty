using System;

namespace OpenLoyalty.Api.Models
{
    public class CreateEarningRuleDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string EventKey { get; set; }
        public int Priority { get; set; }
        public object? Condition { get; set; }
        public object Points { get; set; }
        public object? Limits { get; set; }
        public object? TimeWindow { get; set; }
        public string[]? Segments { get; set; }
        
        // Added missing scheduling properties
        public DateTime? ActivateAt { get; set; }
        public DateTime? DeactivateAt { get; set; }
        public string? CronExpression { get; set; }
    }

    public class EarningRuleListItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string EventKey { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
    }
}
