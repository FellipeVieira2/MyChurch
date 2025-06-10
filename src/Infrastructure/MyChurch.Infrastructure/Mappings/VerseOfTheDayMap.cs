using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class VerseOfTheDayConfiguration : IEntityTypeConfiguration<VerseOfTheDay>
    {
        public void Configure(EntityTypeBuilder<VerseOfTheDay> builder)
        {
            builder.ToTable("verses_of_the_day", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.VerseText)
                .HasColumnName("verse_text")
                .IsRequired();

            builder.Property(x => x.Reference)
                .HasColumnName("reference")
                .IsRequired();

            builder.Property(x => x.Date)
                .HasColumnName("date")
                .IsRequired();
        }
    }
}