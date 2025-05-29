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
        public Church Church { get; set; }
        public bool RequiresParticipantList { get; set; } = false;
        public ICollection<Member> Participants { get; set; } = new List<Member>();
        public EventRecurrence Recurrence { get; set; }
        public ICollection<EventNotification> Notifications { get; set; } = new List<EventNotification>();
    }
}
