using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class FeedPostConfiguration : IEntityTypeConfiguration<FeedPost>
    {
        public void Configure(EntityTypeBuilder<FeedPost> builder)
        {
            builder.ToTable("feed_posts", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnName("content")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("updated");

            builder.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId);

            builder.HasOne(x => x.Church)
                .WithMany()
                .HasForeignKey(x => x.ChurchId);

            builder.HasMany(x => x.Likes)
                .WithOne(x => x.FeedPost)
                .HasForeignKey(x => x.FeedPostId);
        }
    }
}
