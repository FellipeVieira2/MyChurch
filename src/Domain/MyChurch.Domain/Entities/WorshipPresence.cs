namespace MyChurch.Domain.Entities
{
    public class WorshipPresence
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public int? MemberId { get; set; }
        public int? VisitorId { get; set; } // visitante anônimo
        public DateTime Timestamp { get; set; }
        public double? Latitude { get; set; } // localização do check-in
        public double? Longitude { get; set; } // localização do check-in
    }
}
