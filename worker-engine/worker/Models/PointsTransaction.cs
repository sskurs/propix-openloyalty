using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worker.Models
{
    [Table("points_transactions")]
    public class PointsTransaction
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("member_id")]
        public string MemberId { get; set; } = "";

        [Required]
        [Column("transaction_type")]
        public string TransactionType { get; set; } = ""; // EARNED, REDEEMED, EXPIRED, ADJUSTED

        [Column("points")]
        public decimal Points { get; set; }

        [Column("balance_after")]
        public decimal BalanceAfter { get; set; }

        [Column("campaign_id")]
        public string? CampaignId { get; set; }

        [Column("order_id")]
        public string? OrderId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("metadata", TypeName = "jsonb")]
        public string? Metadata { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
