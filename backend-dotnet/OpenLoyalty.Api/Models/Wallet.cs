using System;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Api.Models
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; }
        public string Type { get; set; }
        public decimal Balance { get; set; }
        public string? Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
