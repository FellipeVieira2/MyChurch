namespace MyChurch.Domain.Entities.Bible
{
    public class Hymn
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Language { get; set; }
        public string Lyrics { get; set; }
        public string LyricsAuthor { get; set; }
        public string MelodyAuthor { get; set; }
    }
}