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

            builder
                .HasOne(a => a.Church)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
