using System;

namespace OpenLoyalty.Api.Models
{
    public class CampaignExecution
    {
        public Guid Id { get; set; }
        public string RuleId { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime ExecutedAt { get; set; }
        public string? RewardType { get; set; }
        public decimal? RewardValue { get; set; }
    }
}
