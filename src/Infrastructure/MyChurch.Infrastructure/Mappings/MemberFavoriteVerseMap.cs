using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberFavoriteVerseMap : IEntityTypeConfiguration<MemberFavoriteVerse>
    {
        public void Configure(EntityTypeBuilder<MemberFavoriteVerse> builder)
        {
            builder.ToTable("member_favorite_verses");
            
            builder.HasKey(mfv => mfv.Id);

            builder.Property(mfv => mfv.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(mfv => mfv.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(mfv => mfv.VersionId)
                .HasColumnName("version_id")
                .IsRequired();

            builder.Property(mfv => mfv.BookName)
                .HasColumnName("book_name")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(mfv => mfv.ChapterNumber)
                .HasColumnName("chapter_number")
                .IsRequired();

            builder.Property(mfv => mfv.VerseNumber)
                .HasColumnName("verse_number")
                .IsRequired();

            builder.Property(mfv => mfv.DateFavorited)
                .HasColumnName("date_favorited")
                .IsRequired();

            // Define foreign key relationship with Member
            builder.HasOne(mfv => mfv.Member)
                .WithMany()
                .HasForeignKey(mfv => mfv.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create a unique index to prevent duplicates for the same verse reference
            builder.HasIndex(mfv => new { mfv.MemberId, mfv.VersionId, mfv.BookName, mfv.ChapterNumber, mfv.VerseNumber })
                .IsUnique()
                .HasName("ix_member_favorite_verses_unique_verse");
        }
    }
}