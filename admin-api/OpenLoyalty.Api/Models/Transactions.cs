using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Api.Models
{
    [Table("transactions")]
    public class Transactions
    {
        [Key]
        [Column("id")]
        public required string Id { get; set; } // e.g. "t-{guid}"

        [Required]
        [Column("customer_id")]
        public required string CustomerId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; } // monetary value

        [Column("occurred_at")]
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
