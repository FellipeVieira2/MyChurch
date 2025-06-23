using System;
using System.Collections.Generic;

namespace MyChurch.Application.Dtos
{
    public class BibleReadingPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationInDays { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public int? ChurchId { get; set; }
        public DateTime Created { get; set; }
        public List<BibleReadingPlanStageDto> Stages { get; set; } = new List<BibleReadingPlanStageDto>();
        public int TotalStages => Stages?.Count ?? 0;
    }
}