using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class DepartmentMap : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("departments", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.BankingInfoId)
                .HasColumnName("banking_info_id");

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("updated");

            builder.HasIndex(x => new { x.ChurchId, x.Name }).IsUnique();

            builder.HasOne(x => x.Church)
                .WithMany()
                .HasForeignKey(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BankingInfo)
                .WithMany()
                .HasForeignKey(x => x.BankingInfoId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Members)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
