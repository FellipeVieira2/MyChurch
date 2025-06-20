using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class JourneyStageDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public JourneyStageType Type { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int FaithPointsAwarded { get; set; }
        public bool IsCompleted { get; set; }

        public static JourneyStageDto Create(JourneyStage stage, List<int> completedStageIds)
        {
            return new JourneyStageDto
            {
                Id = stage.Id,
                Title = stage.Title,
                Type = stage.Type,
                Content = stage.Content,
                Order = stage.Order,
                FaithPointsAwarded = stage.FaithPointsAwarded,
                IsCompleted = completedStageIds.Contains(stage.Id)
            };
        }
    }
}
