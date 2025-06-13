namespace MyChurch.Domain.Entities
{
    public class WorshipActivity
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public string Name { get; set; } = string.Empty; // Ex: Leitura, Louvor, Oferta, etc.
        public string? Content { get; set; } // Texto livre, instruções, etc.
        public int Order { get; set; }
        public bool IsCurrent { get; set; }

        // Relacionamentos
        public WorshipService WorshipService { get; set; }
        public ICollection<WorshipActivityBible> Bibles { get; set; } = new List<WorshipActivityBible>();
        public ICollection<WorshipActivityHymn> Hymns { get; set; } = new List<WorshipActivityHymn>();
    }
}