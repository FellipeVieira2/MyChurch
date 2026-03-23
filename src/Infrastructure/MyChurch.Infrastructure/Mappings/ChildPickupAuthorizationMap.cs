using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChildPickupAuthorizationMap : IEntityTypeConfiguration<ChildPickupAuthorization>
    {
        public void Configure(EntityTypeBuilder<ChildPickupAuthorization> builder)
        {
            builder.ToTable("child_pickup_authorizations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChildId)
                .HasColumnName("child_id")
                .HasColumnType("integer")
                .IsRequired();

            builder.Property(x => x.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Relationship)
                .HasColumnName("relationship")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.DocumentNumber)
                .HasColumnName("document_number")
                .HasMaxLength(30);

            builder.Property(x => x.PhoneNumber)
                .HasColumnName("phone_number")
                .HasMaxLength(20);

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.Property(x => x.CreatedByMemberId)
                .HasColumnName("created_by_member_id")
                .HasColumnType("int");

            builder.HasIndex(x => x.ChildId);
            builder.HasIndex(x => new { x.ChildId, x.FullName, x.PhoneNumber });

            builder.HasOne(x => x.Child)
                .WithMany(x => x.PickupAuthorizations)
                .HasForeignKey(x => x.ChildId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedByMember)
                .WithMany()
                .HasForeignKey(x => x.CreatedByMemberId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
