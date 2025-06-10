
namespace MyChurch.Domain.Entities
{
    public class VerseOfTheDay
    {
        public int Id { get; set; }
        public string VerseText { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}