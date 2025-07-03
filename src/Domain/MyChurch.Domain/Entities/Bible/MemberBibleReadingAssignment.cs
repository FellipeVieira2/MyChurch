using System;

namespace MyChurch.Domain.Entities.Bible
{
    public class MemberBibleReadingAssignment
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int BibleReadingPlanId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
