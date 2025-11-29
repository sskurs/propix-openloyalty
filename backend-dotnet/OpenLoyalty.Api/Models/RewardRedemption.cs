using System;

namespace OpenLoyalty.Api.Models
{
    public class RewardRedemption
    {
        public Guid Id { get; set; }
        public Guid RewardId { get; set; }
        public Reward Reward { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public Guid? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public decimal? PointsSpent { get; set; }
        public decimal? CashbackSpent { get; set; }
        public int Quantity { get; set; } = 1;
        public string Status { get; set; } = "completed";
        public Guid? ChannelId { get; set; }
        public Channel? Channel { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
