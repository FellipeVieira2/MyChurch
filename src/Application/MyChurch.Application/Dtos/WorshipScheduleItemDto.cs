namespace MyChurch.Application.Dtos
{
    public class WorshipScheduleItemDto
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }

        public static WorshipScheduleItemDto New(Domain.Entities.WorshipScheduleItem entity)
        {
            return new WorshipScheduleItemDto
            {
                Id = entity.Id,
                WorshipServiceId = entity.WorshipServiceId,
                Name = entity.Name,
                Order = entity.Order
            };
        }
    }
}
