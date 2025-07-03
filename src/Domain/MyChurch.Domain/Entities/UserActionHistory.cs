using System;

namespace MyChurch.Domain.Entities
{
    public class UserActionHistory
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? ActionData { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
