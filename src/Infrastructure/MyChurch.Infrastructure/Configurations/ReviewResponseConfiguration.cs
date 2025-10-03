using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class ReviewResponseConfiguration : IEntityTypeConfiguration<ReviewResponse>
    {
        public void Configure(EntityTypeBuilder<ReviewResponse> builder)
        {
            builder.ToTable("ReviewResponses");
            
            builder.HasKey(rr => rr.Id);
            
            builder.Property(rr => rr.Response)
                .IsRequired()
                .HasMaxLength(1000);
            
            builder.Property(rr => rr.RespondedAt)
                .IsRequired();
            
            builder.Property(rr => rr.IsEdited)
                .IsRequired()
                .HasDefaultValue(false);
            
            // Relacionamento com Review (1:1)
            builder.HasOne(rr => rr.Review)
                .WithOne(r => r.OfficialResponse)
                .HasForeignKey<ReviewResponse>(rr => rr.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacionamento com Member (Responder)
            builder.HasOne(rr => rr.Responder)
                .WithMany()
                .HasForeignKey(rr => rr.ResponderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Índices
            builder.HasIndex(rr => rr.ReviewId)
                .IsUnique() // Uma review pode ter apenas uma resposta oficial
                .HasDatabaseName("IX_ReviewResponses_ReviewId");
            
            builder.HasIndex(rr => rr.ResponderId)
                .HasDatabaseName("IX_ReviewResponses_ResponderId");
            
            builder.HasIndex(rr => rr.RespondedAt)
                .HasDatabaseName("IX_ReviewResponses_RespondedAt");
        }
    }
}
