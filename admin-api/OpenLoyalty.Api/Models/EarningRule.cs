using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Api.Models
{
    [Table("earning_rules")]
    public class EarningRule
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public required string Category { get; set; }

        [Required]
        public required string EventKey { get; set; }

        [Required]
        public string Status { get; set; } = "DRAFT";

        public int Priority { get; set; } = 100;

        public int Version { get; set; } = 1;

        [Required]
        public string Type { get; set; } = "earning";

        public DateTime? ActivateAt { get; set; }
        public DateTime? DeactivateAt { get; set; }
        public string? CronExpression { get; set; }

        public string? ConditionJson { get; set; }
        public string? PointsJson { get; set; }
        public string? LimitsJson { get; set; }
        public string? TimeWindowJson { get; set; }
        public string? SegmentsJson { get; set; }
        public string? Metadata { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
