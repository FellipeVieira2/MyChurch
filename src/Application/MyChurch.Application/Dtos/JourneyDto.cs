using MyChurch.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MyChurch.Application.Dtos
{
    public class JourneyDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
        public double Progress { get; set; }
        public List<JourneyStageDto> Stages { get; set; } = new List<JourneyStageDto>();

        public static JourneyDto Create(Domain.Entities.Journey journey, List<int> completedStageIds)
        {
            var completedCount = journey.Stages.Count(s => completedStageIds.Contains(s.Id));
            var totalStages = journey.Stages.Count;
            var progress = totalStages > 0 ? (double)completedCount / totalStages * 100 : 0;

            return new JourneyDto
            {
                Id = journey.Id,
                Title = journey.Title,
                Description = journey.Description,
                IconUrl = journey.IconUrl,
                IsActive = journey.IsActive,
                Progress = progress,
                Stages = journey.Stages.Select(s => JourneyStageDto.Create(s, completedStageIds)).OrderBy(s => s.Order).ToList()
            };
        }
    }
}
