using System;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Api.Models
{
    public class Campaign
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Status { get; set; } // DRAFT, ACTIVE, PAUSED, COMPLETED

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
