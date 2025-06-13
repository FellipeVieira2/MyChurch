namespace MyChurch.Domain.Entities
{
    public class WorshipPresence
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public int MemberId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
