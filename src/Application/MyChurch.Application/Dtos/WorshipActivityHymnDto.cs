using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class WorshipActivityHymnDto
    {
        public int Id { get; set; }
        public int HymnId { get; set; }
        public string? HymnTitle { get; set; }
        public string? HymnNumber { get; set; }

        public static WorshipActivityHymnDto New(WorshipActivityHymn entity)
        {
            return new WorshipActivityHymnDto
            {
                Id = entity.Id,
                HymnId = entity.HymnId,
                HymnTitle = entity.HymnTitle,
                HymnNumber = entity.HymnNumber
            };
        }
    }
}
