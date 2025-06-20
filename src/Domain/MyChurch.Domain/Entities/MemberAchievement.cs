using System;

namespace MyChurch.Domain.Entities
{
    public class MemberAchievement
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int AchievementId { get; set; }
        public DateTime AwardedAt { get; set; }
    }
}
