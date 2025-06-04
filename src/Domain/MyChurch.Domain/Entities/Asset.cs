using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Value { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public AssetType Type { get; set; }
        public string IdentificationCode { get; set; } = null!;
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Novos campos
        public string Condition { get; set; } = null!; // Condição do ativo (ex: Novo, Usado, Danificado)
        public DateTime? PurchaseDate { get; set; } // Data de compra
        public string Location { get; set; } = null!; // Localização física
        public string Responsible { get; set; } = null!; // Responsável pelo ativo
        public DateTime? LastMaintenance { get; set; } // Última manutenção
        public DateTime? NextMaintenance { get; set; } // Próxima manutenção
        public DateTime? WarrantyUntil { get; set; } // Garantia até
        public string Notes { get; set; } = null!; // Observações

        public void Update(
           string name,
           decimal value,
           int quantity,
           string description,
           string photo,
           AssetType type,
           string identificationCode,
           string condition,
           DateTime? purchaseDate,
           string location,
           string responsible,
           DateTime? lastMaintenance,
           DateTime? nextMaintenance,
           DateTime? warrantyUntil,
           string notes)
        {
            Name = name;
            Value = value;
            Quantity = quantity;
            Description = description;
            Photo = photo;
            Type = type;
            IdentificationCode = identificationCode;
            Condition = condition;
            PurchaseDate = purchaseDate;
            Location = location;
            Responsible = responsible;
            LastMaintenance = lastMaintenance;
            NextMaintenance = nextMaintenance;
            WarrantyUntil = warrantyUntil;
            Notes = notes;
        }
    }
}
