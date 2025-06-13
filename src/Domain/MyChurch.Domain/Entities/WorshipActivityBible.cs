namespace MyChurch.Domain.Entities
{
    public class WorshipActivityBible
    {
        public int Id { get; set; }
        public int WorshipActivityId { get; set; }
        public int BibleVersionId { get; set; }
        public int BookId { get; set; }
        public int ChapterId { get; set; }
        public int VerseStart { get; set; }
        public int? VerseEnd { get; set; }

        // Navegação
        public WorshipActivity WorshipActivity { get; set; }
    }
}