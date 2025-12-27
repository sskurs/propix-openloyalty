using System;

namespace OpenLoyalty.Customer.Api.Models
{
    public class MemberCoupon
    {
        public Guid Id { get; set; }
        public required string MemberId { get; set; } // Matches TransactionEvent UserId (String)
        public required string CouponCode { get; set; }
        public required string Status { get; set; } // "active", "redeemed", "expired"
        public DateTime AssignedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
    }
}
