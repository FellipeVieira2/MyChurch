using System;
using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class EngagementEvent
    {
        public Guid Id { get; set; }
        public int MemberId { get; set; } // Corrigido para int
        public int ChurchId { get; set; }
        public int Points { get; set; }
        public EngagementEventType EventType { get; set; }
        public string EventReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Member Member { get; set; }
    }
}
