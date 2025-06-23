using System.Collections.Generic;

namespace MyChurch.Application.Dtos
{
    public class BibleReadingPlanProgressDto
    {
        public BibleReadingPlanDto Plan { get; set; }
        public int CompletedStages { get; set; }
        public int TotalStages { get; set; }
        public decimal ProgressPercentage => TotalStages > 0 ? (decimal)CompletedStages / TotalStages * 100 : 0;
        public BibleReadingPlanStageDto NextStage { get; set; }
        public List<BibleReadingPlanStageDto> Stages { get; set; } = new List<BibleReadingPlanStageDto>();
    }
}