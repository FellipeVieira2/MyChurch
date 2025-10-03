using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class ReviewVoteConfiguration : IEntityTypeConfiguration<ReviewVote>
    {
        public void Configure(EntityTypeBuilder<ReviewVote> builder)
        {
            builder.ToTable("ReviewVotes");
            
            builder.HasKey(rv => rv.Id);
            
            builder.Property(rv => rv.IsHelpful)
                .IsRequired();
            
            builder.Property(rv => rv.CreatedAt)
                .IsRequired();
            
            // Relacionamento com Review
            builder.HasOne(rv => rv.Review)
                .WithMany(r => r.Votes)
                .HasForeignKey(rv => rv.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamento com Member
            builder.HasOne(rv => rv.Member)
                .WithMany()
                .HasForeignKey(rv => rv.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Índice único para garantir um voto por membro por review
            builder.HasIndex(rv => new { rv.ReviewId, rv.MemberId })
                .IsUnique()
                .HasDatabaseName("IX_ReviewVotes_ReviewId_MemberId");
            
            // Índices para performance
            builder.HasIndex(rv => rv.ReviewId)
                .HasDatabaseName("IX_ReviewVotes_ReviewId");
            
            builder.HasIndex(rv => rv.MemberId)
                .HasDatabaseName("IX_ReviewVotes_MemberId");
        }
    }
}
