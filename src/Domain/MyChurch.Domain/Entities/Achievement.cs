using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Achievement
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public AchievementType Type { get; set; }
        public int Threshold { get; set; }
    }
}
