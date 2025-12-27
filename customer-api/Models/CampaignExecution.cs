using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Customer.Api.Models
{
    [Table("campaign_executions")]
    public class CampaignExecution
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("campaign_id")]
        public Guid CampaignId { get; set; }

        [Column("member_id")]
        public string? MemberId { get; set; }

        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [Column("reward_type")]
        public string? RewardType { get; set; }

        [Column("execution_result_json", TypeName = "jsonb")]
        public string? ExecutionResultJson { get; set; }

        [Column("executed_at")]
        public DateTime ExecutedAt { get; set; }
    }
}
