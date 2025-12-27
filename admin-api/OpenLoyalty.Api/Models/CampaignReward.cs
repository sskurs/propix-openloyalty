using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Api.Models
{
    [Table("campaign_rewards")]
    public class CampaignReward
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CampaignId { get; set; }
        public required string RewardType { get; set; }
        public required string Value { get; set; } // Store as JSON string

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CampaignId")]
        public Campaign? Campaign { get; set; }
    }
}
