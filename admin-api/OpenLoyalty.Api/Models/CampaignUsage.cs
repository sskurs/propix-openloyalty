using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Api.Models
{
    [Table("campaign_usage")]
    public class CampaignUsage
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CampaignId { get; set; }
        public Guid CustomerId { get; set; }

        public int UsageCount { get; set; } = 0;
        public DateTime? LastUsedAt { get; set; }

        [ForeignKey("CampaignId")]
        public Campaign? Campaign { get; set; }
    }
}
