
namespace MyChurch.Application.Dtos
{
    public class WorshipScheduleItem
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public string Name { get; set; } = string.Empty; // Ex: "Leitura Bíblica", "Louvor das crianças"
        public int Order { get; set; } // Ordem no cronograma
        public static WorshipScheduleItem New(WorshipScheduleItem item)
        {
            return new WorshipScheduleItem
            {
                Id = item.Id,
                WorshipServiceId = item.WorshipServiceId,
                Name = item.Name,
                Order = item.Order
            };
        }
    }
}
