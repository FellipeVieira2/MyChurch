namespace MyChurch.Domain.Entities.Bible
{
    public class Hymn
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Language { get; set; }
        public string Chorus { get; set; } // Novo campo para o coro
        public string LyricsAuthor { get; set; }
        public string MelodyAuthor { get; set; }

        // Relacionamento com os versos
        public ICollection<HymnVerse> HymnVerses { get; set; } = new List<HymnVerse>();
    }
}