using System;
using System.Collections.Generic;

namespace OpenLoyalty.Customer.Api.Models.Dtos
{
    public class MemberProfileDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public string TierName { get; set; } = string.Empty;
        public List<WalletDto> Wallets { get; set; } = new();
    }

    public class WalletDto
    {
        public string Type { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
