namespace MyChurch.Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int ChurchId { get; set; }
        public Church Church { get; set; }
        public ICollection<Member> Participants { get; set; } = new List<Member>();
    }
}
