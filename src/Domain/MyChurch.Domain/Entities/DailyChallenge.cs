using MyChurch.Domain.Enum;
using System;

namespace MyChurch.Domain.Entities
{
    public class DailyChallenge
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public DateTime Date { get; set; }
        public DailyChallengeType Type { get; set; }
        public string Content { get; set; }
        public int FaithPointsAwarded { get; set; }
    }
}
