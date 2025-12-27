using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worker.Models
{
    [Table("campaign_executions")]
    public class CampaignExecutionEntity
    {
        [Key]
        public long Id { get; set; }

        public Guid CampaignId { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        
        public string RewardType { get; set; } = string.Empty; // POINTS, COUPON
        
        [Column(TypeName = "jsonb")]
        public string ExecutionResultJson { get; set; } = "{}"; // Details of what was given

        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
    }
}
