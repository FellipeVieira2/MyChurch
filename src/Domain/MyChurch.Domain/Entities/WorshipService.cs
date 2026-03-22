using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class WorshipService
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Theme { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Description { get; set; }
        public WorshipServiceStatus Status { get; set; } = WorshipServiceStatus.NotStarted;
        public ICollection<WorshipActivity> Activities { get; set; } = new List<WorshipActivity>();
        public ICollection<WorshipPresence> Presences { get; set; } = new List<WorshipPresence>();
        public ICollection<WorshipScheduleItem> Schedule { get; set; } = new List<WorshipScheduleItem>();
        public ICollection<WorshipScaleMember> ScaleMembers { get; set; } = new List<WorshipScaleMember>();
        public ICollection<DonationWorshipService> DonationWorshipServices { get; set; } = new List<DonationWorshipService>();
        public ICollection<PrayerRequest> PrayerRequests { get; set; } = new List<PrayerRequest>();
    }
}
