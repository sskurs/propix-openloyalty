using System;

namespace OpenLoyalty.Api.Models
{
    public class Achievement
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string Criteria { get; set; } // JSON
        public string? RewardRules { get; set; } // JSON
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
