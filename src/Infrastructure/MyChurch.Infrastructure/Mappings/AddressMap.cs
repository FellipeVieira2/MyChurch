using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class AddressMap : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);
            builder.ToTable("address", "postgres");

            builder
                .Property(a => a.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(a => a.Street)
                .HasColumnName("street")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(a => a.City)
                .HasColumnName("city")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(a => a.State)
                .HasColumnName("state")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(a => a.ZipCode)
                .HasColumnName("zip_code")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder
                .Property(a => a.Country)
                .HasColumnName("country")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(a => a.Neighborhood)
                .HasColumnName("neighborhood")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(a => a.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(a => a.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .HasOne(a => a.Church)
                .WithOne(c => c.Address)
                .HasForeignKey<Church>(c => c.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}