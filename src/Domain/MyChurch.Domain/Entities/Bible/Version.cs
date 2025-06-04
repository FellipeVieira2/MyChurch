namespace MyChurch.Domain.Entities.Bible
{
    public class Version
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Language { get; set; }
        public string Description { get; set; }
        public string Publisher { get; set; }
        public int? PublicationYear { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}