using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class WorshipScaleMemberMap : IEntityTypeConfiguration<WorshipScaleMember>
    {
        public void Configure(EntityTypeBuilder<WorshipScaleMember> builder)
        {
            builder.ToTable("worship_scale_members", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WorshipServiceId)
                .HasColumnName("worship_service_id")
                .IsRequired();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(x => x.RoleName)
                .HasColumnName("role_name")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Order)
                .HasColumnName("order")
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasColumnName("notes")
                .HasMaxLength(500);

            builder.HasIndex(x => new { x.WorshipServiceId, x.MemberId, x.RoleName }).IsUnique();

            builder.HasOne(x => x.WorshipService)
                .WithMany(x => x.ScaleMembers)
                .HasForeignKey(x => x.WorshipServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
