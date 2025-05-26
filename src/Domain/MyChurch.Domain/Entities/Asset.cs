using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Value { get; set; }
        public string Description { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public AssetType Type { get; set; }
        public string IdentificationCode { get; set; } = null!;
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public void Update(string name, decimal value, string description, string photo, AssetType type, string identificationCode)
        {
            Name = name;
            Value = value;
            Description = description;
            Photo = photo;
            Type = type;
            IdentificationCode = identificationCode;
        }
    }
}