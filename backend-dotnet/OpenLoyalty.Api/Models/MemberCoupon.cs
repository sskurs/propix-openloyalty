using System;

namespace OpenLoyalty.Api.Models
{
    public class MemberCoupon
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public Guid CouponId { get; set; }
        public Coupon Coupon { get; set; }
        public DateTime AssignedAt { get; set; }
        public int UsedCount { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public string Status { get; set; } = "active";
    }
}
