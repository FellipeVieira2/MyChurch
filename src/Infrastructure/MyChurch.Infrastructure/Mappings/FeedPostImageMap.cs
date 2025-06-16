using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class FeedPostImageConfiguration : IEntityTypeConfiguration<FeedPostImage>
    {
        public void Configure(EntityTypeBuilder<FeedPostImage> builder)
        {
            builder.ToTable("feed_post_images", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.FeedPostId)
                .HasColumnName("feed_post_id")
                .IsRequired();

            builder.Property(x => x.FileName)
                .HasColumnName("file_name")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.HasOne(x => x.FeedPost)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.FeedPostId);
        }
    }
}
