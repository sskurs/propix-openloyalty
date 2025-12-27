using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Api.Models
{
    [Table("campaigns")]
    public class Campaign
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public required string Code { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(30)]
        public required string Status { get; set; } // draft | active | paused | expired | archived

        [StringLength(50)]
        public string? Type { get; set; } // e.g. bonus-points, points-multiplier, cashback
        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public int Priority { get; set; } = 0;
        public bool IsStackable { get; set; } = true;

        public int? MaxTotalRewards { get; set; }
        public int? MaxPerCustomer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [ConcurrencyCheck]
        public byte[]? RowVersion { get; set; }

        // Navigation properties
        public ICollection<CampaignCondition> Conditions { get; set; } = new List<CampaignCondition>();
        public ICollection<CampaignReward> Rewards { get; set; } = new List<CampaignReward>();
        public ICollection<CampaignUsage> Usages { get; set; } = new List<CampaignUsage>();
    }
}
