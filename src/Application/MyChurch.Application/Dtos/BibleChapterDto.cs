namespace MyChurch.Application.Dtos
{
    public class BibleChapterDto
    {
        public int Id { get; set; }
        public int ChapterNumber { get; set; }

        public static BibleChapterDto New(Domain.Entities.Bible.Chapter chapter)
        {
            return new BibleChapterDto
            {
                Id = chapter.Id,
                ChapterNumber = chapter.ChapterNumber
            };
        }
    }
}
