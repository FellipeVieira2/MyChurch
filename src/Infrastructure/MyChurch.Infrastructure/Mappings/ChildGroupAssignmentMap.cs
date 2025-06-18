using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChildGroupAssignmentMap : IEntityTypeConfiguration<ChildGroupAssignment>
    {
        public void Configure(EntityTypeBuilder<ChildGroupAssignment> builder)
        {
            builder.ToTable("child_group_assignments");
            builder.HasKey(cga => new { cga.ChildId, cga.GroupId });
            builder.Property(cga => cga.ChildId).HasColumnName("child_id");
            builder.Property(cga => cga.GroupId).HasColumnName("group_id");
            builder.HasOne(cga => cga.Child).WithMany(c => c.GroupAssignments).HasForeignKey(cga => cga.ChildId);
            builder.HasOne(cga => cga.Group).WithMany().HasForeignKey(cga => cga.GroupId);
        }
    }
}
