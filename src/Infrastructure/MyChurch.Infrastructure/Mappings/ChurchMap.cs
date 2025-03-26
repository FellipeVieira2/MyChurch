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
