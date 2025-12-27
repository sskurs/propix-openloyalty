using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoyalty.Customer.Api.Models
{
    [Table("member_points")]
    public class MemberPoints
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("member_id")]
        public string MemberId { get; set; } = "";

        [Column("points_balance")]
        public decimal PointsBalance { get; set; }

        [Column("lifetime_points")]
        public decimal LifetimePoints { get; set; }

        [Column("points_earned_this_month")]
        public decimal PointsEarnedThisMonth { get; set; }

        [Column("points_redeemed")]
        public decimal PointsRedeemed { get; set; }

        [Column("tier")]
        public string Tier { get; set; } = "BRONZE";

        [Column("last_updated_at")]
        public DateTime LastUpdatedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
