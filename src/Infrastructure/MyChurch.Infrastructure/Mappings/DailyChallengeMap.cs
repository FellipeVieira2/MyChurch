using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class DailyChallengeMap : IEntityTypeConfiguration<DailyChallenge>
    {
        public void Configure(EntityTypeBuilder<DailyChallenge> builder)
        {
            builder.ToTable("daily_challenges");

            builder.HasKey(dc => dc.Id);

            builder.Property(dc => dc.Id).HasColumnName("id");
            builder.Property(dc => dc.ChurchId).HasColumnName("church_id").IsRequired();
            builder.Property(dc => dc.Date).HasColumnName("date").IsRequired();
            builder.Property(dc => dc.Type).HasColumnName("type").IsRequired();
            builder.Property(dc => dc.Content).HasColumnName("content").IsRequired();
            builder.Property(dc => dc.FaithPointsAwarded).HasColumnName("faith_points_awarded").IsRequired();
        }
    }
}
