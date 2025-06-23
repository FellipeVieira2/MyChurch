using System;

namespace MyChurch.Application.Dtos
{
    public class MemberBibleReadingProgressDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int BibleReadingPlanId { get; set; }
        public int BibleReadingPlanStageId { get; set; }
        public DateTime DateCompleted { get; set; }
        public string StageName { get; set; }
        public int StageOrder { get; set; }
        public string VerseReferences { get; set; }
    }
}