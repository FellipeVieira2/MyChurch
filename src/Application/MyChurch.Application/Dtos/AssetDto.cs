using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class AssetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Value { get; set; }
        public string Description { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public AssetType Type { get; set; }
        public string IdentificationCode { get; set; } = null!;
        public int ChurchId { get; set; }
        public DateTime CreatedAt { get; set; }

        public static AssetDto New(Domain.Entities.Asset asset)
        {
            return new AssetDto
            {
                Id = asset.Id,
                Name = asset.Name,
                Value = asset.Value,
                Description = asset.Description,
                Photo = asset.Photo,
                Type = asset.Type,
                IdentificationCode = asset.IdentificationCode,
                ChurchId = asset.ChurchId,
                CreatedAt = asset.CreatedAt
            };
        }
    }
}
