using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberJourneyProgressMap : IEntityTypeConfiguration<MemberJourneyProgress>
    {
        public void Configure(EntityTypeBuilder<MemberJourneyProgress> builder)
        {
            builder.ToTable("member_journey_progresses");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.MemberId).HasColumnName("member_id").IsRequired();
            builder.Property(p => p.JourneyStageId).HasColumnName("journey_stage_id").IsRequired();
            builder.Property(p => p.CompletedAt).HasColumnName("completed_at").IsRequired();
        }
    }
}
