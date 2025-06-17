namespace MyChurch.Application.Dtos
{
    public class HymnDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Language { get; set; }
        public string Chorus { get; set; }
        public string LyricsAuthor { get; set; }
        public string MelodyAuthor { get; set; }
        public List<HymnVerseDto> Verses { get; set; }
    }

    public class HymnVerseDto
    {
        public int Number { get; set; }
        public string Text { get; set; }
    }
}