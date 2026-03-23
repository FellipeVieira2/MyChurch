namespace MyChurch.Application.Dtos
{
    public class HymnSummaryDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string LyricsAuthor { get; set; }
        public string MelodyAuthor { get; set; }
        public bool HasChorus { get; set; }
        public int VerseCount { get; set; }
    }
}
