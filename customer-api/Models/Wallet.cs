using System;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Customer.Api.Models
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public required string Type { get; set; }
        public decimal Balance { get; set; }
        public string? Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
