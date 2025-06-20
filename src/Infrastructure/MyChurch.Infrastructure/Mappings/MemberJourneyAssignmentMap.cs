using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberJourneyAssignmentMap : IEntityTypeConfiguration<MemberJourneyAssignment>
    {
        public void Configure(EntityTypeBuilder<MemberJourneyAssignment> builder)
        {
            builder.ToTable("member_journey_assignments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.MemberId).HasColumnName("member_id").IsRequired();
            builder.Property(a => a.JourneyId).HasColumnName("journey_id").IsRequired();
            builder.Property(a => a.AssignedAt).HasColumnName("assigned_at").IsRequired();
        }
    }
}
