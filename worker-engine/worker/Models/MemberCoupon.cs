using System;

namespace Worker.Models
{
    public class MemberCoupon
    {
        public Guid Id { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public DateTime AssignedAt { get; set; }
        public DateTime? RedeemedAt { get; set; }
    }
}
