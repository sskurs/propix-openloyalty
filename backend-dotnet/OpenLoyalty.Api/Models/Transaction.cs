using System;

namespace OpenLoyalty.Api.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid? MemberId { get; set; }
        public Member? Member { get; set; }
        public string? ExternalId { get; set; }
        public string Type { get; set; }
        public Guid? StoreId { get; set; }
        public Store? Store { get; set; }
        public Guid? ChannelId { get; set; }
        public Channel? Channel { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; } = "completed";
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
