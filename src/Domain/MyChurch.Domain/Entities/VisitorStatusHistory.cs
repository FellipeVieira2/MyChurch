using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class VisitorStatusHistory
    {
        public int Id { get; set; }
        public int VisitorId { get; set; }
        public Visitor Visitor { get; set; }
        public VisitorStatus OldStatus { get; set; }
        public VisitorStatus NewStatus { get; set; }
        public int? ChangedByMemberId { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }
    }
}
