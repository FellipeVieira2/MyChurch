using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberBibleReadingAssignmentMap : IEntityTypeConfiguration<MemberBibleReadingAssignment>
    {
        public void Configure(EntityTypeBuilder<MemberBibleReadingAssignment> builder)
        {
            builder.ToTable("member_bible_reading_assignment", "postgres");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasColumnName("id").HasColumnType("int").ValueGeneratedOnAdd().IsRequired();
            builder.Property(a => a.MemberId).HasColumnName("member_id").HasColumnType("int").IsRequired();
            builder.Property(a => a.BibleReadingPlanId).HasColumnName("bible_reading_plan_id").HasColumnType("int").IsRequired();
            builder.Property(a => a.AssignedAt).HasColumnName("assigned_at").HasColumnType("timestamp").IsRequired();

            builder.HasIndex(a => new { a.MemberId, a.BibleReadingPlanId }).IsUnique();
        }
    }
}
