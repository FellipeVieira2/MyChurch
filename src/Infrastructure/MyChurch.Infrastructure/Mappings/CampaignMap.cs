using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class CampaignMap : IEntityTypeConfiguration<Campaign>
    {
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("campaigns");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.GoalAmount)
                .HasColumnName("goal_amount")
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.AmountRaised)
                .HasColumnType("decimal(18,2)")
                .HasColumnName("amount_raised");

            builder.Property(c => c.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.CoverImageUrl)
                .HasColumnName("cover_image_url")
                .IsRequired(false)
                .HasMaxLength(500);

            builder
                .Property(x => x.StartDate)
                .HasColumnName("start_date")
                .IsRequired();

            builder
                .Property(x => x.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder
                .Property(x => x.IsCompleted)
                .HasColumnName("is_completed")
                .IsRequired();

            builder
                .Property(c => c.EndDate)
                .HasColumnName("end_date")
                .IsRequired();

            builder
                .Property(c => c.ChurchId)
                .HasColumnName("church_id")
                .HasMaxLength(300);


            builder.HasOne(c => c.Church)
                   .WithMany()
                   .HasForeignKey(c => c.ChurchId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
