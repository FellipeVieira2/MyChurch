using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class AssetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Value { get; set; }
        public int Quantity { get; set; }
        public decimal TotalValue { get; set; }
        public string Description { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public AssetType Type { get; set; }
        public string IdentificationCode { get; set; } = null!;
        public int ChurchId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Condition { get; set; } = null!;
        public DateTime? PurchaseDate { get; set; }
        public string Location { get; set; } = null!;
        public string Responsible { get; set; } = null!;
        public DateTime? LastMaintenance { get; set; }
        public DateTime? NextMaintenance { get; set; }
        public DateTime? WarrantyUntil { get; set; }
        public string Notes { get; set; } = null!;

        public static AssetDto New(Domain.Entities.Asset asset)
        {
            return new AssetDto
            {
                Id = asset.Id,
                Name = asset.Name,
                Value = asset.Value,
                Quantity = asset.Quantity,
                TotalValue = asset.Value * asset.Quantity,
                Description = asset.Description,
                Photo = asset.Photo,
                Type = asset.Type,
                IdentificationCode = asset.IdentificationCode,
                ChurchId = asset.ChurchId,
                CreatedAt = asset.CreatedAt,
                Condition = asset.Condition,
                PurchaseDate = asset.PurchaseDate,
                Location = asset.Location,
                Responsible = asset.Responsible,
                LastMaintenance = asset.LastMaintenance,
                NextMaintenance = asset.NextMaintenance,
                WarrantyUntil = asset.WarrantyUntil,
                Notes = asset.Notes
            };
        }
    }
}