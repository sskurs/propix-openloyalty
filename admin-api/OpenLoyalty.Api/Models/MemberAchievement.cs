using System;

namespace OpenLoyalty.Api.Models
{
    public class MemberAchievement
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public Guid AchievementId { get; set; }
        public Achievement Achievement { get; set; } = null!;
        public DateTime UnlockedAt { get; set; }
        public string Status { get; set; } = "unlocked";
    }
}
