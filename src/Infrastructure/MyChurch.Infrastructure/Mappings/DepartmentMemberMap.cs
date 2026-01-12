using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class DepartmentMemberMap : IEntityTypeConfiguration<DepartmentMember>
    {
        public void Configure(EntityTypeBuilder<DepartmentMember> builder)
        {
            builder.ToTable("department_members", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.DepartmentId)
                .HasColumnName("department_id")
                .IsRequired();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(x => x.Role)
                .HasColumnName("role")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.JoinedAt)
                .HasColumnName("joined_at")
                .IsRequired();

            builder.HasIndex(x => new { x.DepartmentId, x.MemberId }).IsUnique();

            builder.HasOne(x => x.Department)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
