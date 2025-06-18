using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class GroupMap : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("group");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id).HasColumnName("id");
            builder.Property(g => g.ChurchId).HasColumnName("church_id");
            builder.Property(g => g.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            builder.Property(g => g.Description).HasColumnName("description").HasMaxLength(500);
            builder.Property(g => g.Type).HasColumnName("type");
            builder.Property(g => g.LeaderId).HasColumnName("leader_id");
            builder.Property(g => g.IsActive).HasColumnName("is_active");
            builder.Property(g => g.AcceptsNewMembers).HasColumnName("accepts_new_members");
            builder.Property(g => g.CoverImageUrl).HasColumnName("cover_image_url");
            builder.Property(g => g.CreatedAt).HasColumnName("created_at");
            builder.HasOne(g => g.Leader).WithMany().HasForeignKey(g => g.LeaderId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
