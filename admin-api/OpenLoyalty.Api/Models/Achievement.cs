using System;

namespace OpenLoyalty.Api.Models
{
    public class Achievement
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public required string Criteria { get; set; } // JSON
        public string? RewardRules { get; set; } // JSON
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
