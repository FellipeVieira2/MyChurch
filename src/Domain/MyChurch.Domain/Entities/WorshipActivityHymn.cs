
namespace MyChurch.Domain.Entities
{
    public class WorshipActivityHymn
    {
        public int Id { get; set; }
        public int WorshipActivityId { get; set; }
        public int HymnId { get; set; } // FK para tabela de hinos
        public string? HymnTitle { get; set; }
        public string? HymnNumber { get; set; }
        public int VerseNumber { get; set; }

        // Navegação
        public WorshipActivity WorshipActivity { get; set; }
    }
}