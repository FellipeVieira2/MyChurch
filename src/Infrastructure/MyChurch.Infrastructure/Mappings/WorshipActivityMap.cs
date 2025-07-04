using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class WorshipActivityMap : IEntityTypeConfiguration<WorshipActivity>
    {
        public void Configure(EntityTypeBuilder<WorshipActivity> builder)
        {
            builder.ToTable("worship_activities", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WorshipServiceId)
                .HasColumnName("worship_service_id")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(x => x.Content)
                .HasColumnName("content");

            builder.Property(x => x.Order)
                .HasColumnName("order")
                .IsRequired();

            builder
                .Property(x => x.DonationTime)
                .HasColumnName("donation_time")
                .IsRequired();

            builder.Property(x => x.IsCurrent)
                .HasColumnName("is_current")
                .IsRequired();

            builder.HasMany(x => x.Bibles)
                .WithOne(x => x.WorshipActivity)
                .HasForeignKey(x => x.WorshipActivityId);

            builder.HasMany(x => x.Hymns)
                .WithOne(x => x.WorshipActivity)
                .HasForeignKey(x => x.WorshipActivityId);

            builder.HasOne(x => x.WorshipService)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.WorshipServiceId);
        }
    }
}