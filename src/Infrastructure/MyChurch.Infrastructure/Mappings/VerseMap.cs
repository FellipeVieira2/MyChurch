using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class VerseMap : IEntityTypeConfiguration<Verse>
    {
        public void Configure(EntityTypeBuilder<Verse> builder)
        {
            builder.ToTable("verses", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChapterId)
                .HasColumnName("chapter_id")
                .IsRequired();

            builder.Property(x => x.VerseNumber)
                .HasColumnName("verse_number")
                .IsRequired();

            builder.Property(x => x.Text)
                .HasColumnName("text")
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne(x => x.Chapter)
                .WithMany(c => c.Verses)
                .HasForeignKey(x => x.ChapterId);
        }
    }
}