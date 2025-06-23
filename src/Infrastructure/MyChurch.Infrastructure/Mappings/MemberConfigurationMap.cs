using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class MemberConfigurationMap : IEntityTypeConfiguration<MemberConfiguration>
    {
        public void Configure(EntityTypeBuilder<MemberConfiguration> builder)
        {
            builder.ToTable("member_configurations");
            
            builder.HasKey(mc => mc.Id);

            builder.Property(mc => mc.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(mc => mc.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(mc => mc.PreferredBibleVersionId)
                .HasColumnName("preferred_bible_version_id");

            builder.Property(mc => mc.ThemePreference)
                .HasColumnName("theme_preference")
                .HasMaxLength(20)
                .HasDefaultValue("Light")
                .IsRequired();

            builder.Property(mc => mc.FontSize)
                .HasColumnName("font_size")
                .HasMaxLength(10)
                .HasDefaultValue("Medium")
                .IsRequired();

            builder.Property(mc => mc.EnableNotifications)
                .HasColumnName("enable_notifications")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(mc => mc.LastUpdated)
                .HasColumnName("last_updated")
                .IsRequired();

            // Define foreign key relationship with Member
            builder.HasOne(mc => mc.Member)
                .WithOne()
                .HasForeignKey<MemberConfiguration>(mc => mc.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define foreign key relationship with Version
            builder.HasOne(mc => mc.PreferredBibleVersion)
                .WithMany()
                .HasForeignKey(mc => mc.PreferredBibleVersionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Create a unique index to ensure one configuration per member
            builder.HasIndex(mc => mc.MemberId)
                .IsUnique()
                .HasName("ix_member_configurations_member_id_unique");
        }
    }
}