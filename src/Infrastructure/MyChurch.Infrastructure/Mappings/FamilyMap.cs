using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class FamilyMap : IEntityTypeConfiguration<Family>
    {
        public void Configure(EntityTypeBuilder<Family> builder)
        {
            builder.ToTable("families");
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).HasColumnName("id");
            builder.Property(f => f.ChurchId).HasColumnName("church_id");
            builder.Property(f => f.FamilyName).HasColumnName("family_name").IsRequired().HasMaxLength(100);
            builder.Property(f => f.CreatedAt).HasColumnName("created_at");
        }
    }
}
