using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class CashFlowCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int ChurchId { get; set; }
        public string? ChurchName { get; set; }

        public static CashFlowCategoryDto New(CashFlowCategory category)
        {
            return new CashFlowCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ChurchId = category.ChurchId,
                ChurchName = category.Church?.Name
            };
        }
    }
}
