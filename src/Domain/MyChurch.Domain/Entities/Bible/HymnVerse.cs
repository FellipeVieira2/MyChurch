namespace MyChurch.Domain.Entities.Bible
{
    public class HymnVerse
    {
        public int Id { get; set; }
        public int HymnId { get; set; } // FK para Hymn
        public int Number { get; set; } // Número do verso
        public string Text { get; set; }
        public Hymn Hymn { get; set; } // Navegação para Hymn
    }
}
