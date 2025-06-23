using System;

namespace MyChurch.Domain.Entities.Bible
{
    public class MemberBibleReadingProgress
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int BibleReadingPlanId { get; set; }
        public BibleReadingPlan BibleReadingPlan { get; set; }

        public int BibleReadingPlanStageId { get; set; }
        public BibleReadingPlanStage BibleReadingPlanStage { get; set; }

        public DateTime DateCompleted { get; set; } = DateTime.UtcNow;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}