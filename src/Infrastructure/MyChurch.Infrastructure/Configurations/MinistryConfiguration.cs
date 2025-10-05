using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class MinistryConfiguration : IEntityTypeConfiguration<Ministry>
    {
        public void Configure(EntityTypeBuilder<Ministry> builder)
        {
            builder.ToTable("Ministries");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.Photo)
                .HasMaxLength(500);

            builder.Property(m => m.Color)
                .HasMaxLength(50);

            builder.Property(m => m.MeetingDay)
                .HasMaxLength(50);

            builder.Property(m => m.MeetingLocation)
                .HasMaxLength(200);

            builder.Property(m => m.Created)
                .IsRequired();

            builder.Property(m => m.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Relacionamento com Church
            builder.HasOne(m => m.Church)
                .WithMany()
                .HasForeignKey(m => m.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com Leader (Member)
            builder.HasOne(m => m.Leader)
                .WithMany()
                .HasForeignKey(m => m.LeaderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices
            builder.HasIndex(m => m.ChurchId);
            builder.HasIndex(m => m.LeaderId);
            builder.HasIndex(m => m.IsActive);
        }
    }
}
