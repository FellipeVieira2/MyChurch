using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class BibleReadingPlanMap : IEntityTypeConfiguration<BibleReadingPlan>
    {
        public void Configure(EntityTypeBuilder<BibleReadingPlan> builder)
        {
            builder.ToTable("bible_reading_plan", "postgres");

            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(p => p.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .Property(p => p.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(500)")
                .IsRequired();

            builder
                .Property(p => p.DurationInDays)
                .HasColumnName("duration_in_days")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.IsDefault)
                .HasColumnName("is_default")
                .HasColumnType("boolean")
                .IsRequired();

            builder
                .Property(p => p.IsPublic)
                .HasColumnName("is_public")
                .HasColumnType("boolean")
                .IsRequired();

            builder
                .Property(p => p.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired(false);

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
                .HasOne(p => p.Church)
                .WithMany()
                .HasForeignKey(p => p.ChurchId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder
                .HasMany(p => p.BibleReadingPlanStages)
                .WithOne(s => s.BibleReadingPlan)
                .HasForeignKey(s => s.BibleReadingPlanId);

            builder
                .HasMany(p => p.MemberBibleReadingProgresses)
                .WithOne(s => s.BibleReadingPlan)
                .HasForeignKey(s => s.BibleReadingPlanId);
        }
    }
}