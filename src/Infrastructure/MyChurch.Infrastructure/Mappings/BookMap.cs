using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities.Bible;

namespace MyChurch.Infrastructure.Mappings
{
    public class BookMap : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books", "postgres");

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

            builder.Property(x => x.Order)
                .HasColumnName("order")
                .IsRequired();

            builder.Property(x => x.Testament)
                .HasColumnName("testament")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.VersionId)
                .HasColumnName("version_id")
                .IsRequired();

            builder.HasOne(x => x.Version)
                .WithMany(v => v.Books)
                .HasForeignKey(x => x.VersionId);

            builder.HasMany(x => x.Chapters)
                .WithOne(c => c.Book)
                .HasForeignKey(c => c.BookId);
        }
    }
}