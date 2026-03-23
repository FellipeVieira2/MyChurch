using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime FinishDate { get; set; }
        public string Location { get; set; }
        public int ChurchId { get; set; }
        public EventType EventType { get; set; }
        public Church Church { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public bool RequiresParticipantList { get; set; } = false;
        public ICollection<Member> Participants { get; set; } = new List<Member>();
        public ICollection<DiaconateScaleMember> DiaconateScaleMembers { get; set; } = new List<DiaconateScaleMember>();
        public ICollection<KidsScaleMember> KidsScaleMembers { get; set; } = new List<KidsScaleMember>();
        public ICollection<WorshipService> WorshipServices { get; set; } = new List<WorshipService>();
        public EventRecurrence Recurrence { get; set; }
        public ICollection<EventNotification> Notifications { get; set; } = new List<EventNotification>();
    }
}
