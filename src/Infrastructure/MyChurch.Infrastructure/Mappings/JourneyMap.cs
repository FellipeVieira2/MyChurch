using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class JourneyMap : IEntityTypeConfiguration<Journey>
    {
        public void Configure(EntityTypeBuilder<Journey> builder)
        {
            builder.ToTable("journeys");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Id).HasColumnName("id");
            builder.Property(j => j.ChurchId).HasColumnName("church_id").IsRequired();
            builder.Property(j => j.Title).HasColumnName("title").IsRequired().HasMaxLength(100);
            builder.Property(j => j.Description).HasColumnName("description").IsRequired();
            builder.Property(j => j.IconUrl).HasColumnName("icon_url");
            builder.Property(j => j.IsActive).HasColumnName("is_active").IsRequired();
            builder.Property(j => j.IsDefault).HasColumnName("is_default").IsRequired();

            builder.HasMany(j => j.Stages)
                .WithOne(s => s.Journey)
                .HasForeignKey(s => s.JourneyId);
        }
    }
}
