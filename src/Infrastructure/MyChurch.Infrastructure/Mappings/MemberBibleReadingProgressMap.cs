using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberBibleReadingProgressMap : IEntityTypeConfiguration<MemberBibleReadingProgress>
    {
        public void Configure(EntityTypeBuilder<MemberBibleReadingProgress> builder)
        {
            builder.ToTable("member_bible_reading_progress", "postgres");

            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(p => p.MemberId)
                .HasColumnName("member_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.BibleReadingPlanId)
                .HasColumnName("bible_reading_plan_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.BibleReadingPlanStageId)
                .HasColumnName("bible_reading_plan_stage_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.DateCompleted)
                .HasColumnName("date_completed")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(p => p.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(p => p.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .HasOne(p => p.Member)
                .WithMany()
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.BibleReadingPlan)
                .WithMany(p => p.MemberBibleReadingProgresses)
                .HasForeignKey(p => p.BibleReadingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.BibleReadingPlanStage)
                .WithMany()
                .HasForeignKey(p => p.BibleReadingPlanStageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add a unique constraint for member_id + stage_id to prevent duplicate entries
            builder
                .HasIndex(p => new { p.MemberId, p.BibleReadingPlanStageId })
                .IsUnique();
        }
    }
}