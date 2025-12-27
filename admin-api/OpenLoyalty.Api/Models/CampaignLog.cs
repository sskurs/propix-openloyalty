using System;

namespace OpenLoyalty.Api.Models
{
    public class CampaignLog
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public Campaign Campaign { get; set; } = null!;
        public Guid MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public Guid? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public string? EventType { get; set; }
        public string? RewardType { get; set; }
        public decimal? RewardValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
