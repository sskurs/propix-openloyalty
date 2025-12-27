using System;

namespace OpenLoyalty.Api.Models
{
    public class Reward
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public int? StockQty { get; set; }
        public decimal? CostPoints { get; set; }
        public decimal? CostCashback { get; set; }
        public Guid? MinTierId { get; set; }
        public Tier? MinTier { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
