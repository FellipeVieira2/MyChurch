using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class HymnMap : IEntityTypeConfiguration<Hymn>
    {
        public void Configure(EntityTypeBuilder<Hymn> builder)
        {
            builder.ToTable("hymns", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Number)
                .HasColumnName("number")
                .IsRequired();

            builder.Property(x => x.Language)
                .HasColumnName("language")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Chorus)
                .HasColumnName("chorus")
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(x => x.LyricsAuthor)
                .HasColumnName("lyrics_author")
                .HasMaxLength(100);

            builder.Property(x => x.MelodyAuthor)
                .HasColumnName("melody_author")
                .HasMaxLength(100);

            builder.HasMany(x => x.HymnVerses)
                .WithOne(v => v.Hymn)
                .HasForeignKey(v => v.HymnId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}