using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class DonationMap : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("donation", "postgres");

            builder.HasKey(d => d.Id);

            builder
                .Property(d => d.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(d => d.MemberId)
                .HasColumnName("member_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .Property(d => d.VisitorId)
                .HasColumnName("visitor_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .Property(d => d.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder.Property(d => d.PlatformFee)
                .HasColumnName("platform_fee")
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(d => d.Date)
                .HasColumnName("date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .HasOne(d => d.Member)
                .WithMany(m => m.Donations)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(d => d.Visitor)
                .WithMany()
                .HasForeignKey(d => d.VisitorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasMany(d => d.DonationWorshipServices)
                .WithOne(dws => dws.Donation)
                .HasForeignKey(dws => dws.DonationId);

            // Adiciona relacionamento com Campaign
            builder
                .Property(d => d.CampaignId)
                .HasColumnName("campaign_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .HasOne(d => d.Campaign)
                .WithMany()
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
