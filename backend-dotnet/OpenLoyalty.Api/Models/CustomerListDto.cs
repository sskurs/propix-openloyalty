using System;

namespace OpenLoyalty.Api.Models
{
    public class CustomerListDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public int PointsBalance { get; set; }
        public string LoyaltyTierName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; } // Added
    }
}
