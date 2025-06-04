namespace MyChurch.Domain.Entities.Bible
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public int Order { get; set; }
        public string Testament { get; set; }
        public int VersionId { get; set; }
        public Version Version { get; set; }

        public ICollection<Chapter> Chapters { get; set; }
    }
}