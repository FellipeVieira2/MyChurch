using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class MemberCustomPermissionConfiguration : IEntityTypeConfiguration<MemberCustomPermission>
    {
        public void Configure(EntityTypeBuilder<MemberCustomPermission> builder)
        {
            builder.ToTable("member_custom_permissions");

            builder.HasKey(mcp => mcp.Id);

            builder.Property(mcp => mcp.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(mcp => mcp.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(mcp => mcp.Permission)
                .HasColumnName("permission")
                .IsRequired();

            builder.Property(mcp => mcp.IsGranted)
                .HasColumnName("is_granted")
                .IsRequired();

            builder.Property(mcp => mcp.GrantedByMemberId)
                .HasColumnName("granted_by_member_id");

            builder.Property(mcp => mcp.Reason)
                .HasColumnName("reason")
                .HasMaxLength(500);

            builder.Property(mcp => mcp.ExpiresAt)
                .HasColumnName("expires_at")
                .HasColumnType("timestamp without time zone");

            builder.Property(mcp => mcp.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp without time zone")
                .IsRequired();

            builder.Property(mcp => mcp.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp without time zone");

            // Relacionamentos
            builder.HasOne(mcp => mcp.Member)
                .WithMany()
                .HasForeignKey(mcp => mcp.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mcp => mcp.GrantedByMember)
                .WithMany()
                .HasForeignKey(mcp => mcp.GrantedByMemberId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices
            builder.HasIndex(mcp => new { mcp.MemberId, mcp.Permission })
                .HasDatabaseName("ix_member_custom_permissions_member_permission")
                .IsUnique();

            builder.HasIndex(mcp => mcp.MemberId)
                .HasDatabaseName("ix_member_custom_permissions_member_id");

            builder.HasIndex(mcp => mcp.ExpiresAt)
                .HasDatabaseName("ix_member_custom_permissions_expires_at");
        }
    }
}
