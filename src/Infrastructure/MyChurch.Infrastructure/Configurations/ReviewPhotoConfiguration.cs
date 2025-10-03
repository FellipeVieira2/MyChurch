using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class ReviewPhotoConfiguration : IEntityTypeConfiguration<ReviewPhoto>
    {
        public void Configure(EntityTypeBuilder<ReviewPhoto> builder)
        {
            builder.ToTable("ReviewPhotos");
            
            builder.HasKey(rp => rp.Id);
            
            builder.Property(rp => rp.PhotoUrl)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(rp => rp.OriginalFileName)
                .HasMaxLength(255);
            
            builder.Property(rp => rp.Caption)
                .HasMaxLength(500);
            
            builder.Property(rp => rp.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);
            
            builder.Property(rp => rp.UploadedAt)
                .IsRequired();
            
            // Relacionamento com Review
            builder.HasOne(rp => rp.Review)
                .WithMany(r => r.Photos)
                .HasForeignKey(rp => rp.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Índices
            builder.HasIndex(rp => rp.ReviewId)
                .HasDatabaseName("IX_ReviewPhotos_ReviewId");
            
            builder.HasIndex(rp => rp.UploadedAt)
                .HasDatabaseName("IX_ReviewPhotos_UploadedAt");
        }
    }
}
