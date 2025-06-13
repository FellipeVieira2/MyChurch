using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PrayerRequestMap : IEntityTypeConfiguration<PrayerRequest>
    {
        public void Configure(EntityTypeBuilder<PrayerRequest> builder)
        {
            builder.ToTable("prayer_request", "postgres");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").IsRequired();
            builder.Property(x => x.WorshipServiceId).HasColumnName("worship_service_id").IsRequired();
            builder.Property(x => x.MemberId).HasColumnName("member_id").IsRequired();
            builder.Property(x => x.Request).HasColumnName("request").IsRequired();
            builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(x => x.IsRead).HasColumnName("is_read").IsRequired();
            builder.HasOne(x => x.WorshipService)
                .WithMany(ws => ws.PrayerRequests)
                .HasForeignKey(x => x.WorshipServiceId);
            builder.HasOne(x => x.Member)
                .WithMany(m => m.PrayerRequests)
                .HasForeignKey(x => x.MemberId);
        }
    }
}
