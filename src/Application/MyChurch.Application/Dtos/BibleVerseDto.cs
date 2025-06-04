namespace MyChurch.Application.Dtos
{
    public class BibleVerseDto
    {
        public int Id { get; set; }
        public int VerseNumber { get; set; }
        public string Text { get; set; }
        public static BibleVerseDto New(Domain.Entities.Bible.Verse verse)
        {
            return new BibleVerseDto
            {
                Id = verse.Id,
                VerseNumber = verse.VerseNumber,
                Text = verse.Text
            };
        }
    }
}
