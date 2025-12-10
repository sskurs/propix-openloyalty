using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Api.Models
{
    public class Member
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public DateTime JoinDate { get; set; }
        public string Status { get; set; }
        public Guid TierId { get; set; }
        public Tier Tier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ExternalId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public ICollection<Wallet> Wallets { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
