using System;
using System.Collections.Generic;

namespace OpenLoyalty.Api.Models
{
    // DTO for wallet details to prevent cycles
    public class WalletDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string? Currency { get; set; }
    }

    // DTO for detailed member view to prevent cycles
    public class MemberDetailsDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ExternalId { get; set; }
        public string? TierName { get; set; }
        public ICollection<WalletDto> Wallets { get; set; } = new List<WalletDto>();
    }

    public class MemberUsageHistoryDto
    {
        public Guid Id { get; set; }
        public string? CampaignName { get; set; }
        public int UsageCount { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }

    public class MemberRedemptionHistoryDto
    {
        public Guid Id { get; set; }
        public string? RewardName { get; set; }
        public decimal? PointsSpent { get; set; }
        public decimal? CashbackSpent { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
