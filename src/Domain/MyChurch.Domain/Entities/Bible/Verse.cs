namespace MyChurch.Domain.Entities.Bible
{
    public class Verse
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int VerseNumber { get; set; }
        public string Text { get; set; }
        public Chapter Chapter { get; set; }
    }
}