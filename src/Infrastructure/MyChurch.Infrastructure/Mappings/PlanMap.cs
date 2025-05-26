using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PlanMap : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTable("plan", "postgres");

            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(p => p.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(p => p.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder
                .Property(p => p.MaxMembers)
                .HasColumnName("max_members")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.MaxEvents)
                .HasColumnName("max_events")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.Branches)
                .HasColumnName("branches")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.MaxStorageGB)
                .HasColumnName("max_storage_gb")
                .HasColumnType("int")
                .IsRequired();

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
        }
    }
}
