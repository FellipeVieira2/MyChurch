using System;

namespace MyChurch.Domain.Entities
{
    public class MemberJourneyProgress
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int JourneyStageId { get; set; }
        public JourneyStage JourneyStage { get; set; }
        public DateTime CompletedAt { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}
