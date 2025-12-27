using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worker.Models
{
    [Table("campaigns")]
    public class Campaign
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public required string Code { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(30)]
        public required string Status { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public int Priority { get; set; } = 0;
        public bool IsStackable { get; set; } = true;

        public int? MaxTotalRewards { get; set; }
        public int? MaxPerCustomer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public List<CampaignConditionModel> Conditions { get; set; } = new();

        [NotMapped]
        public List<CampaignRewardModel> Rewards { get; set; } = new();

        [NotMapped]
        public object? RuleGroup { get; set; }
    }

    public class CampaignConditionModel
    {
        public string Type { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public object? Value { get; set; }
        public string? RuleGroupJson { get; set; }
    }

    public class CampaignRewardModel
    {
        public string Type { get; set; } = string.Empty;
        public object? Value { get; set; }
    }
}
