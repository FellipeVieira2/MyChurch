using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class WorshipActivityBibleMap : IEntityTypeConfiguration<WorshipActivityBible>
    {
        public void Configure(EntityTypeBuilder<WorshipActivityBible> builder)
        {
            builder.ToTable("worship_activity_bibles", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WorshipActivityId)
                .HasColumnName("worship_activity_id")
                .IsRequired();

            builder.Property(x => x.BibleVersionId)
                .HasColumnName("bible_version_id")
                .IsRequired();

            builder.Property(x => x.BookId)
                .HasColumnName("book_id")
                .IsRequired();

            builder.Property(x => x.ChapterId)
                .HasColumnName("chapter_id")
                .IsRequired();

            builder.Property(x => x.VerseStart)
                .HasColumnName("verse_start")
                .IsRequired();

            builder.Property(x => x.VerseEnd)
                .HasColumnName("verse_end");

            builder.HasOne(x => x.WorshipActivity)
                .WithMany(x => x.Bibles)
                .HasForeignKey(x => x.WorshipActivityId);
        }
    }
}