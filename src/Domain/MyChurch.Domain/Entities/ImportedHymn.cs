using System.Collections.Generic;

namespace MyChurch.Domain.Entities
{
    public class ImportedHymn
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Lyrics { get; set; } // Letra completa (opcional)
        public ICollection<ImportedHymnStanza> Stanzas { get; set; } = new List<ImportedHymnStanza>();
    }

    public class ImportedHymnStanza
    {
        public int Id { get; set; }
        public int ImportedHymnId { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = string.Empty;
        public ImportedHymn ImportedHymn { get; set; }
    }
}
