using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PresentationMap : IEntityTypeConfiguration<Presentation>
    {
        public void Configure(EntityTypeBuilder<Presentation> builder)
        {
            builder.ToTable("presentations");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(200).HasColumnName("name");
            builder.Property(p => p.Description).HasMaxLength(500).HasColumnName("description");
            builder.Property(p => p.CurrentSlideIndex).HasColumnName("current_slide_index");
            builder.Property(p => p.IsLive).HasColumnName("is_live");
            builder.Property(p => p.ChurchId).HasColumnName("church_id");
            builder.Property(p => p.AdminUserId).HasColumnName("admin_user_id");

            builder.HasOne(p => p.Church)
                .WithMany()
                .HasForeignKey(p => p.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.AdminUser)
                .WithMany()
                .HasForeignKey(p => p.AdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Slides)
                .WithOne(s => s.Presentation)
                .HasForeignKey(s => s.PresentationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
