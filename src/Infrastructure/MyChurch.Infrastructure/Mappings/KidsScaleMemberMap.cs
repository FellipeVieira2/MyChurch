using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class KidsScaleMemberMap : IEntityTypeConfiguration<KidsScaleMember>
    {
        public void Configure(EntityTypeBuilder<KidsScaleMember> builder)
        {
            builder.ToTable("kids_scale_members", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.EventId)
                .HasColumnName("event_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.RoleName)
                .HasColumnName("role_name")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Order)
                .HasColumnName("order")
                .HasColumnType("integer")
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500);

            builder.HasIndex(x => new { x.EventId, x.MemberId, x.RoleName }).IsUnique();

            builder.HasOne(x => x.Event)
                .WithMany(x => x.KidsScaleMembers)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
