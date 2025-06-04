using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChapterMap : IEntityTypeConfiguration<Chapter>
    {
        public void Configure(EntityTypeBuilder<Chapter> builder)
        {
            builder.ToTable("chapters", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.BookId)
                .HasColumnName("book_id")
                .IsRequired();

            builder.Property(x => x.ChapterNumber)
                .HasColumnName("chapter_number")
                .IsRequired();

            builder.HasOne(x => x.Book)
                .WithMany(b => b.Chapters)
                .HasForeignKey(x => x.BookId);

            builder.HasMany(x => x.Verses)
                .WithOne(v => v.Chapter)
                .HasForeignKey(v => v.ChapterId);
        }
    }
}