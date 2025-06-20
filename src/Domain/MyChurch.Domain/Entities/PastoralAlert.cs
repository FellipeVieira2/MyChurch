using System;

namespace MyChurch.Domain.Entities
{
    public class PastoralAlert
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public int MemberId { get; set; }
        public int? LeaderId { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
