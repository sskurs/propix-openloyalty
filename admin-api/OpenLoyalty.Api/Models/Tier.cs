using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenLoyalty.Api.Models
{
    public class Tier
    {
        [Key]
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public decimal Multiplier { get; set; } = 1.0m;
        public decimal ThresholdValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
