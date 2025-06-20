using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class JourneyStageMap : IEntityTypeConfiguration<JourneyStage>
    {
        public void Configure(EntityTypeBuilder<JourneyStage> builder)
        {
            builder.ToTable("journey_stages");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.JourneyId).HasColumnName("journey_id").IsRequired();
            builder.Property(s => s.Title).HasColumnName("title").IsRequired().HasMaxLength(100);
            builder.Property(s => s.Type).HasColumnName("type").IsRequired();
            builder.Property(s => s.Content).HasColumnName("content").IsRequired();
            builder.Property(s => s.Order).HasColumnName("order").IsRequired();
            builder.Property(s => s.FaithPointsAwarded).HasColumnName("faith_points_awarded").IsRequired();
        }
    }
}
