using System;

namespace MyChurch.Domain.Entities
{
    public class MemberJourneyAssignment
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int JourneyId { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
