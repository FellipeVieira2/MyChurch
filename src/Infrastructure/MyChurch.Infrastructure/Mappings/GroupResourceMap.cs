using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class GroupResourceMap : IEntityTypeConfiguration<GroupResource>
    {
        public void Configure(EntityTypeBuilder<GroupResource> builder)
        {
            builder.ToTable("group_resource");
            builder.HasKey(gr => gr.Id);
            builder.Property(gr => gr.Id).HasColumnName("id");
            builder.Property(gr => gr.GroupId).HasColumnName("group_id");
            builder.Property(gr => gr.Title).HasColumnName("title").IsRequired().HasMaxLength(100);
            builder.Property(gr => gr.Description).HasColumnName("description").HasMaxLength(500);
            builder.Property(gr => gr.FileUrl).HasColumnName("file_url").IsRequired();
            builder.Property(gr => gr.UploadedByMemberId).HasColumnName("uploaded_by_member_id");
            builder.Property(gr => gr.UploadedAt).HasColumnName("uploaded_at");
        }
    }
}
