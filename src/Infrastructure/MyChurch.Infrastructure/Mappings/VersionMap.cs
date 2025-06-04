using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;
using Version = MyChurch.Domain.Entities.Bible.Version;

namespace MyChurch.Infrastructure.Mappings
{
    public class VersionMap : IEntityTypeConfiguration<Version>
    {
        public void Configure(EntityTypeBuilder<Version> builder)
        {
            builder.ToTable("versions", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Abbreviation)
                .HasColumnName("abbreviation")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Language)
                .HasColumnName("language")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            builder.Property(x => x.Publisher)
                .HasColumnName("publisher")
                .HasMaxLength(100);

            builder.Property(x => x.PublicationYear)
                .HasColumnName("publication_year");

            builder.HasMany(x => x.Books)
                .WithOne(b => b.Version)
                .HasForeignKey(b => b.VersionId);
        }
    }
}