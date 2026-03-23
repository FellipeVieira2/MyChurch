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
        public int Id { get; set; }
        public int Number { get; set; }
        public string Text { get; set; }
    }

    public class HymnUpsertDto
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Language { get; set; }
        public string Chorus { get; set; }
        public string LyricsAuthor { get; set; }
        public string MelodyAuthor { get; set; }
        public List<HymnVerseInputDto> Verses { get; set; } = new();
    }

    public class HymnVerseInputDto
    {
        public int Number { get; set; }
        public string Text { get; set; }
    }

    public class HymnPresentationPreviewDto
    {
        public string Title { get; set; }
        public List<HymnPreviewSlideDto> Slides { get; set; } = new();
    }

    public class HymnPreviewSlideDto
    {
        public int Order { get; set; }
        public string Section { get; set; }
        public int? VerseNumber { get; set; }
        public bool IsChorus { get; set; }
        public string DisplayText { get; set; }
        public int CharacterCount { get; set; }
        public int LineCount { get; set; }
        public string PreviewImageBase64 { get; set; }
    }
}