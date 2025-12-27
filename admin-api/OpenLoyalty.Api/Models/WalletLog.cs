using System;

namespace OpenLoyalty.Api.Models
{
    public class WalletLog
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; } = null!;
        public Guid MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public Guid? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public required string Direction { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public required string ReasonCode { get; set; }
        public required string WalletType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
