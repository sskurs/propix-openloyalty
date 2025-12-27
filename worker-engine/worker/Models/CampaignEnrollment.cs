using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worker.Models
{
    [Table("campaign_enrollments")]
    public class CampaignEnrollment
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("campaign_id")]
        public string CampaignId { get; set; } = "";

        [Required]
        [Column("member_id")]
        public string MemberId { get; set; } = "";

        [Column("enrolled_at")]
        public DateTime EnrolledAt { get; set; }

        [Column("status")]
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, COMPLETED, CANCELLED

        [Column("progress", TypeName = "jsonb")]
        public string? Progress { get; set; }

        [Column("rewards_earned", TypeName = "jsonb")]
        public string? RewardsEarned { get; set; }
    }
}
