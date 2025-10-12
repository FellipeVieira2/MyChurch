using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("role_permissions");

            builder.HasKey(rp => rp.Id);

            builder.Property(rp => rp.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(rp => rp.Role)
                .HasColumnName("role")
                .IsRequired();

            builder.Property(rp => rp.Permission)
                .HasColumnName("permission")
                .IsRequired();

            builder.Property(rp => rp.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            builder.Property(rp => rp.ChurchId)
                .HasColumnName("church_id");

            builder.Property(rp => rp.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            builder.Property(rp => rp.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp without time zone");

            // Relacionamentos
            builder.HasOne(rp => rp.Church)
                .WithMany()
                .HasForeignKey(rp => rp.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            builder.HasIndex(rp => new { rp.Role, rp.Permission, rp.ChurchId })
                .HasDatabaseName("ix_role_permissions_role_permission_church");

            builder.HasIndex(rp => rp.ChurchId)
                .HasDatabaseName("ix_role_permissions_church_id");
        }
    }
}
