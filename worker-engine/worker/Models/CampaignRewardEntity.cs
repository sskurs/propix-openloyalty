using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worker.Models
{
    [Table("campaign_rewards")]
    public class CampaignRewardEntity
    {
        [Key]
        public long Id { get; set; }

        public Guid CampaignId { get; set; }

        [Column(TypeName = "jsonb")]
        public string RewardJson { get; set; } = "{}";
    }
}
