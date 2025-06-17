using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class HymnVerseMap : IEntityTypeConfiguration<HymnVerse>
    {
        public void Configure(EntityTypeBuilder<HymnVerse> builder)
        {
            builder.ToTable("hymn_verses", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.HymnId)
                .HasColumnName("hymn_id")
                .IsRequired();

            builder.Property(x => x.Number)
                .HasColumnName("number")
                .IsRequired();

            builder.Property(x => x.Text)
                .HasColumnName("text")
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne(x => x.Hymn)
                .WithMany(h => h.HymnVerses)
                .HasForeignKey(x => x.HymnId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
