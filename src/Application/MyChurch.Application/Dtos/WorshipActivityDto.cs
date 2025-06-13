using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class WorshipActivityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public int Order { get; set; }
        public bool IsCurrent { get; set; }
        public List<WorshipActivityBibleDto> Bibles { get; set; } = new();
        public List<WorshipActivityHymnDto> Hymns { get; set; } = new();

        public static WorshipActivityDto New(Domain.Entities.WorshipActivity entity)
        {
            return new WorshipActivityDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Content = entity.Content,
                Order = entity.Order,
                IsCurrent = entity.IsCurrent,
                Bibles = entity.Bibles?.Select(WorshipActivityBibleDto.New).ToList() ?? new(),
                Hymns = entity.Hymns?.Select(WorshipActivityHymnDto.New).ToList() ?? new()
            };
        }
    }
}
