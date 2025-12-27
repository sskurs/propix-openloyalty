using System;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Customer.Api.Models
{
    public class Campaign
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public required string Code { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }
        
        public string? Description { get; set; }

        [Required]
        public required string Status { get; set; } 

        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }

        public int Priority { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
