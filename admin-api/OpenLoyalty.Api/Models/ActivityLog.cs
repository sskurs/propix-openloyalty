using System;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Api.Models
{
    public class ActivityLog
    {
        [Key]
        public Guid Id { get; set; }
        public required string Type { get; set; } // e.g., "New Member", "Redemption", "Tier Upgrade"
        public required string Description { get; set; }
        public string? Variant { get; set; } // e.g., "destructive", "secondary"
        public DateTime CreatedAt { get; set; }
    }
}
