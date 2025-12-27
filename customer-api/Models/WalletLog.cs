using System;
using System.Text.Json.Serialization;

namespace OpenLoyalty.Customer.Api.Models
{
    public class WalletLog
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        [JsonIgnore]
        public Wallet Wallet { get; set; } = null!;
        public Guid MemberId { get; set; }
        [JsonIgnore]
        public Member Member { get; set; } = null!;
        public Guid? TransactionId { get; set; }
        public string Direction { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string ReasonCode { get; set; } = null!;
        public string WalletType { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
