using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class DonationWorshipServiceMap : IEntityTypeConfiguration<DonationWorshipService>
    {
        public void Configure(EntityTypeBuilder<DonationWorshipService> builder)
        {
            builder.ToTable("donation_worship_service", "postgres");
            builder.HasKey(x => new { x.DonationId, x.WorshipServiceId });

            builder.Property(x => x.DonationId).HasColumnName("donation_id").IsRequired();
            builder.Property(x => x.WorshipServiceId).HasColumnName("worship_service_id").IsRequired();

            builder.HasOne(x => x.Donation)
                .WithMany(d => d.DonationWorshipServices)
                .HasForeignKey(x => x.DonationId);

            builder.HasOne(x => x.WorshipService)
                .WithMany(ws => ws.DonationWorshipServices)
                .HasForeignKey(x => x.WorshipServiceId);
        }
    }
}
