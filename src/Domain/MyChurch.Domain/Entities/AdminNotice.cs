using System;

namespace MyChurch.Domain.Entities
{
    public class AdminNotice
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public int? MemberId { get; set; } // quem enviou
        public string Message { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public Church Church { get; set; }
        public Member? Member { get; set; }
    }
}
