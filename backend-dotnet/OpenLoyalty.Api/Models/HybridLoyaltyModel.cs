using System;
using System.Collections.Generic;

namespace OpenLoyalty.Api.Models
{
    // All models are now in a single file to prevent build/caching issues.

    public class Tier
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public string? Color { get; set; }
        public string? Icon { get; set; }
        public string? Description { get; set; }
        public string ProgressionType { get; set; } = "spend";
        public decimal ThresholdValue { get; set; }
        public string TrackingPeriod { get; set; } = "rolling_12m";
        public decimal Multiplier { get; set; } = 1.0m;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Member> Members { get; set; } = new List<Member>();
    }

    public class Member
    {
        public Guid Id { get; set; }
        public string? ExternalId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; } = "active";
        public Guid? TierId { get; set; }
        public Tier? Tier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
        public ICollection<MemberTierHistory> TierHistory { get; set; } = new List<MemberTierHistory>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<TimelineEvent> TimelineEvents { get; set; } = new List<TimelineEvent>();
    }

    public class Wallet
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
        public string? Currency { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    // ... other models ...
}
