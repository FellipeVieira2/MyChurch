using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class DepartmentGeneralLeaderScopeMap : IEntityTypeConfiguration<DepartmentGeneralLeaderScope>
    {
        public void Configure(EntityTypeBuilder<DepartmentGeneralLeaderScope> builder)
        {
            builder.ToTable("department_general_leader_scopes", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ParentChurchId)
                .HasColumnName("parent_church_id")
                .IsRequired();

            builder.Property(x => x.DepartmentId)
                .HasColumnName("department_id")
                .IsRequired();

            builder.Property(x => x.LeaderMemberId)
                .HasColumnName("leader_member_id")
                .IsRequired();

            builder.Property(x => x.BranchChurchId)
                .HasColumnName("branch_church_id")
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("updated");

            builder.HasIndex(x => new { x.ParentChurchId, x.DepartmentId, x.LeaderMemberId, x.BranchChurchId })
                .IsUnique();

            builder.HasOne(x => x.ParentChurch)
                .WithMany()
                .HasForeignKey(x => x.ParentChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.BranchChurch)
                .WithMany()
                .HasForeignKey(x => x.BranchChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.LeaderMember)
                .WithMany()
                .HasForeignKey(x => x.LeaderMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.LeaderMemberId);
            builder.HasIndex(x => x.DepartmentId);
            builder.HasIndex(x => x.BranchChurchId);
        }
    }
}
