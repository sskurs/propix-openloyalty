using System;

namespace OpenLoyalty.Api.Models
{
    public class MemberTierHistory
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public Guid TierId { get; set; }
        public Tier Tier { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? ChangeReason { get; set; }
        public string? Source { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
