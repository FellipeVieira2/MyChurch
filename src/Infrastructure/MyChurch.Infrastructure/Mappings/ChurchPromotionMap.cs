using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChurchPromotionMap : IEntityTypeConfiguration<ChurchPromotion>
    {
        public void Configure(EntityTypeBuilder<ChurchPromotion> builder)
        {
            builder.ToTable("church_promotions", "postgres");

            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(p => p.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.Type)
                .HasColumnName("type")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.StartDate)
                .HasColumnName("start_date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(p => p.EndDate)
                .HasColumnName("end_date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(p => p.AmountPaid)
                .HasColumnName("amount_paid")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder
                .Property(p => p.Status)
                .HasColumnName("status")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.TargetRegion)
                .HasColumnName("target_region")
                .HasColumnType("varchar(100)")
                .IsRequired(false);

            builder
                .Property(p => p.CustomBannerUrl)
                .HasColumnName("custom_banner_url")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder
                .Property(p => p.CustomText)
                .HasColumnName("custom_text")
                .HasColumnType("varchar(500)")
                .IsRequired(false);

            builder
                .Property(p => p.Views)
                .HasColumnName("views")
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(p => p.Clicks)
                .HasColumnName("clicks")
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(p => p.PaymentId)
                .HasColumnName("payment_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .Property(p => p.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(p => p.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);

            // Relacionamentos
            builder
                .HasOne(p => p.Church)
                .WithMany()
                .HasForeignKey(p => p.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.Payment)
                .WithMany()
                .HasForeignKey(p => p.PaymentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices
            builder
                .HasIndex(p => p.ChurchId)
                .HasDatabaseName("IX_church_promotions_church_id");

            builder
                .HasIndex(p => new { p.Status, p.StartDate, p.EndDate })
                .HasDatabaseName("IX_church_promotions_status_dates");

            builder
                .HasIndex(p => p.Type)
                .HasDatabaseName("IX_church_promotions_type");
        }
    }
}
