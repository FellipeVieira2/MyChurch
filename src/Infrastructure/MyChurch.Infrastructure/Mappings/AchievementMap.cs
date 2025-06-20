using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class AchievementMap : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.ToTable("achievements");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasColumnName("id");
            builder.Property(a => a.Title).HasColumnName("title").IsRequired().HasMaxLength(100);
            builder.Property(a => a.Description).HasColumnName("description").IsRequired();
            builder.Property(a => a.IconUrl).HasColumnName("icon_url").IsRequired();
            builder.Property(a => a.Type).HasColumnName("type").IsRequired();
            builder.Property(a => a.Threshold).HasColumnName("threshold").IsRequired();
        }
    }
}
