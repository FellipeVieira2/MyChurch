using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class AssetMap : IEntityTypeConfiguration<Asset>
    {
        public void Configure(EntityTypeBuilder<Asset> builder)
        {
            builder.ToTable("asset", "postgres");

            builder.HasKey(a => a.Id);

            builder
                .Property(a => a.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(a => a.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(a => a.Value)
                .HasColumnName("value")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder
                .Property(a => a.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .IsRequired();

            builder
                .Property(a => a.Photo)
                .HasColumnName("photo")
                .HasColumnType("varchar(500)")
                .IsRequired();

            builder
                .Property(a => a.Type)
                .HasColumnName("type")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(a => a.IdentificationCode)
                .HasColumnName("identification_code")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(a => a.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(a => a.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            // Novos campos
            builder
                .Property(a => a.Condition)
                .HasColumnName("condition")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder
                .Property(a => a.PurchaseDate)
                .HasColumnName("purchase_date")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .Property(a => a.Location)
                .HasColumnName("location")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(a => a.Responsible)
                .HasColumnName("responsible")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(a => a.LastMaintenance)
                .HasColumnName("last_maintenance")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .Property(a => a.NextMaintenance)
                .HasColumnName("next_maintenance")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .Property(a => a.WarrantyUntil)
                .HasColumnName("warranty_until")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .Property(a => a.Notes)
                .HasColumnName("notes")
                .HasColumnType("text")
                .IsRequired();

            builder
                .HasOne(a => a.Church)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
