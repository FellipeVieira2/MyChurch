using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class WorshipActivityBibleDto
    {
        public int Id { get; set; }
        public int BibleVersionId { get; set; }
        public int BookId { get; set; }
        public int ChapterId { get; set; }
        public int VerseStart { get; set; }
        public int? VerseEnd { get; set; }

        public static WorshipActivityBibleDto New(WorshipActivityBible entity)
        {
            return new WorshipActivityBibleDto
            {
                Id = entity.Id,
                BibleVersionId = entity.BibleVersionId,
                BookId = entity.BookId,
                ChapterId = entity.ChapterId,
                VerseStart = entity.VerseStart,
                VerseEnd = entity.VerseEnd
            };
        }
    }
}
