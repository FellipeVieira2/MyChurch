using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class JourneyStage
    {
        public int Id { get; set; }
        public int JourneyId { get; set; }
        public virtual Journey Journey { get; set; }
        public string Title { get; set; }
        public JourneyStageType Type { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int FaithPointsAwarded { get; set; }
        public bool RequiresLeaderVerification { get; set; } = false;
    }
}
