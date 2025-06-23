using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class BibleReadingPlanStageMap : IEntityTypeConfiguration<BibleReadingPlanStage>
    {
        public void Configure(EntityTypeBuilder<BibleReadingPlanStage> builder)
        {
            builder.ToTable("bible_reading_plan_stage", "postgres");

            builder.HasKey(s => s.Id);

            builder
                .Property(s => s.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(s => s.BibleReadingPlanId)
                .HasColumnName("bible_reading_plan_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(s => s.Order)
                .HasColumnName("order")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(s => s.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(s => s.VerseReferences)
                .HasColumnName("verse_references")
                .HasColumnType("varchar(500)")
                .IsRequired();

            builder
                .Property(s => s.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(s => s.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .HasOne(s => s.BibleReadingPlan)
                .WithMany(p => p.BibleReadingPlanStages)
                .HasForeignKey(s => s.BibleReadingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add a unique constraint for plan_id + order
            builder
                .HasIndex(s => new { s.BibleReadingPlanId, s.Order })
                .IsUnique();
        }
    }
}