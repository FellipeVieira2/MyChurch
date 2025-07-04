using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ImportedHymnMap : IEntityTypeConfiguration<ImportedHymn>
    {
        public void Configure(EntityTypeBuilder<ImportedHymn> builder)
        {
            builder.ToTable("imported_hymns");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Title).IsRequired().HasColumnName("title");
            builder.Property(h => h.Author).HasColumnName("author");
            builder.Property(h => h.Lyrics).HasColumnName("lyrics");
            builder.HasMany(h => h.Stanzas)
                .WithOne(s => s.ImportedHymn)
                .HasForeignKey(s => s.ImportedHymnId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ImportedHymnStanzaMap : IEntityTypeConfiguration<ImportedHymnStanza>
    {
        public void Configure(EntityTypeBuilder<ImportedHymnStanza> builder)
        {
            builder.ToTable("imported_hymn_stanzas");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Text).IsRequired().HasColumnName("text");
            builder.Property(s => s.Order).IsRequired().HasColumnName("order");
            builder.Property(s => s.ImportedHymnId).HasColumnName("imported_hymn_id");
        }
    }
}
