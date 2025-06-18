using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChildMap : IEntityTypeConfiguration<Child>
    {
        public void Configure(EntityTypeBuilder<Child> builder)
        {
            builder.ToTable("child");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.FamilyId).HasColumnName("family_id");
            builder.Property(c => c.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(100);
            builder.Property(c => c.BirthDate).HasColumnName("birth_date");
            builder.Property(c => c.Gender).HasColumnName("gender");
            builder.Property(c => c.IsActive).HasColumnName("is_active");
            builder.HasOne(c => c.Family).WithMany(f => f.Children).HasForeignKey(c => c.FamilyId);
        }
    }
}
