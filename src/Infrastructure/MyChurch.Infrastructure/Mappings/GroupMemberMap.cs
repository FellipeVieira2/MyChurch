using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class GroupMemberMap : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            builder.ToTable("group_member");
            builder.HasKey(gm => gm.Id);
            builder.Property(gm => gm.Id).HasColumnName("id");
            builder.Property(gm => gm.GroupId).HasColumnName("group_id");
            builder.Property(gm => gm.MemberId).HasColumnName("member_id");
            builder.Property(gm => gm.RoleInGroup).HasColumnName("role_in_group").HasMaxLength(50);
            builder.Property(gm => gm.DateJoined).HasColumnName("date_joined");
            builder.HasIndex(gm => new { gm.GroupId, gm.MemberId }).IsUnique();
            builder.HasOne(gm => gm.Group).WithMany(g => g.Members).HasForeignKey(gm => gm.GroupId);
            builder.HasOne(gm => gm.Member).WithMany().HasForeignKey(gm => gm.MemberId);
        }
    }
}
