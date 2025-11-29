using System;

namespace OpenLoyalty.Api.Models
{
    public class TimelineEvent
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public string EventType { get; set; }
        public string? Source { get; set; }
        public Guid? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public Guid? WalletLogId { get; set; }
        public WalletLog? WalletLog { get; set; }
        public Guid? TierFromId { get; set; }
        public Tier? TierFrom { get; set; }
        public Guid? TierToId { get; set; }
        public Tier? TierTo { get; set; }
        public string? Description { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
