using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class FeedLikeMap : IEntityTypeConfiguration<FeedLike>
    {
        public void Configure(EntityTypeBuilder<FeedLike> builder)
        {
            builder.ToTable("feed_likes", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.FeedPostId)
                .HasColumnName("feed_post_id")
                .IsRequired();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.HasOne(x => x.FeedPost)
                .WithMany(x => x.Likes)
                .HasForeignKey(x => x.FeedPostId);

            builder.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId);

            // Restrição opcional para evitar likes duplicados por membro no mesmo post
            builder.HasIndex(x => new { x.FeedPostId, x.MemberId })
                .IsUnique();
        }
    }
}
