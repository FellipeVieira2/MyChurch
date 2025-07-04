namespace MyChurch.Application.Dtos
{
    public class ImportedHymnDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Author { get; set; }
        public string? Lyrics { get; set; }
        public List<ImportedHymnStanzaDto> Stanzas { get; set; } = new();
    }

    public class ImportedHymnStanzaDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
    }
}
