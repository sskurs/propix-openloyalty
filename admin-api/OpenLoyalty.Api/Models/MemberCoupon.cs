using System;

namespace OpenLoyalty.Api.Models
{
    public class MemberCoupon
    {
        public Guid Id { get; set; }
        public required string MemberId { get; set; }
        public required string CouponCode { get; set; }
        public required string Status { get; set; } = "active";
        public DateTime AssignedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
    }
}
