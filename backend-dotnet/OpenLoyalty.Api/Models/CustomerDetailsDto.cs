using System;

namespace OpenLoyalty.Api.Models
{
    public class CustomerDetailsDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool HasAgreedToTerms { get; set; }
        public bool IsActive { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public int PointsBalance { get; set; }
        public string LoyaltyTierName { get; set; }
        public string? ReferralCode { get; set; }
        public string? LoyaltyCardNumber { get; set; } // Added
    }
}
