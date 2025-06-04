namespace MyChurch.Domain.Entities.Bible
{
    public class Chapter
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int ChapterNumber { get; set; }

        public Book Book { get; set; }
        public ICollection<Verse> Verses { get; set; }
    }
}