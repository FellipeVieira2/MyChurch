using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChurchMap : IEntityTypeConfiguration<Church>
    {
        public void Configure(EntityTypeBuilder<Church> builder)
        {
            builder.ToTable("church", "postgres");

            builder.HasKey(c => c.Id);
            builder
                .Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .IsRequired();

            builder
                .Property(c => c.PlatformFee)
                .HasColumnName("platform_fee")
                .HasColumnType("decimal(5,4)")
                .HasDefaultValue(0.05m)
                .IsRequired();

            builder
                .Property(x => x.LogoFileName)
                .HasColumnName("logo_file_name")
                .HasColumnType("varchar(200)")
                .IsRequired(false);

            builder
                .Property(x => x.Phone)
                .HasColumnName("phone")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder
                .Property(x => x.AddressId)
                .HasColumnName("address_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(x => x.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(x => x.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .Property(x => x.Document)
                .HasColumnName("document")
                .HasColumnType("varchar(20)")
                .IsRequired(false);

            builder
                .Property(x => x.AsaasCustomerId)
                .HasColumnName("asaas_customer_id")
                .HasColumnType("varchar(50)")
                .IsRequired(false);

            builder
                .Property(x => x.OnboardingQrCode)
                .HasColumnName("onboarding_qrcode")
                .HasColumnType("text")
                .IsRequired(false);

            builder
                .Property(x => x.ParentChurchId)
                .HasColumnName("parent_church_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .HasOne(x => x.ParentChurch)
                .WithMany(x => x.Branches)
                .HasForeignKey(x => x.ParentChurchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasIndex(x => x.ParentChurchId)
                .HasName("IX_Church_ParentChurchId");

            builder
                .HasOne(x => x.Address)
                .WithOne(x => x.Church)
                .HasForeignKey<Church>(x => x.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.Members)
                .WithOne(x => x.Church)
                .HasForeignKey(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.Events)
                .WithOne(x => x.Church)
                .HasForeignKey(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Subscription)
                .WithOne(x => x.Church)
                .HasForeignKey<Subscription>(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
