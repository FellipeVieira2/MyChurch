using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class SlideMap : IEntityTypeConfiguration<Slide>
    {
        public void Configure(EntityTypeBuilder<Slide> builder)
        {
            builder.ToTable("slides");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.PresentationId).HasColumnName("presentation_id");
            builder.Property(s => s.OrderIndex).HasColumnName("order_index");
            builder.Property(s => s.ContentType).HasColumnName("content_type");
            builder.Property(s => s.ContentReferenceJson).HasColumnName("content_reference_json").HasColumnType("jsonb");
            builder.Property(s => s.CachedDisplayText).HasColumnName("cached_display_text");
            builder.Property(s => s.CachedMediaUrl).HasColumnName("cached_media_url");

            builder.HasOne(s => s.Presentation)
                .WithMany(p => p.Slides)
                .HasForeignKey(s => s.PresentationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
