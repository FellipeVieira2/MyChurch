using System;

namespace MyChurch.Domain.Entities
{
    public class PrayerRequest
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public WorshipService WorshipService { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public string Request { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
