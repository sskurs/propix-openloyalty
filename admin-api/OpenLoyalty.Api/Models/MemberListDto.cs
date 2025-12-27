using System;

namespace OpenLoyalty.Api.Models
{
    public class MemberListDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TierName { get; set; }
        public decimal PointsBalance { get; set; }
        public string Status { get; set; } = "active";
        public string? ExternalId { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
