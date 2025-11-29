using System;

namespace OpenLoyalty.Api.Models
{
    public class WalletLog
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public Guid? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
        public string Direction { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string ReasonCode { get; set; }
        public string WalletType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
