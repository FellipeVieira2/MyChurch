using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? BankingInfoId { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public static DepartmentDto New(Department entity)
        {
            return new DepartmentDto
            {
                Id = entity.Id,
                ChurchId = entity.ChurchId,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                BankingInfoId = entity.BankingInfoId,
                Created = entity.Created,
                Updated = entity.Updated
            };
        }
    }
}
