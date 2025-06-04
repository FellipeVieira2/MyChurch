namespace MyChurch.Application.Dtos
{
    public class BibleVersionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Language { get; set; }
        public string? Description { get; set; }
        public string? Publisher { get; set; }
        public int? PublicationYear { get; set; }

        public static BibleVersionDto New(Domain.Entities.Bible.Version version)
        {
            return new BibleVersionDto
            {
                Id = version.Id,
                Name = version.Name,
                Abbreviation = version.Abbreviation,
                Language = version.Language,
                Description = version.Description,
                Publisher = version.Publisher,
                PublicationYear = version.PublicationYear
            };
        }
    }
}