using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class WorshipPresenceMap : IEntityTypeConfiguration<WorshipPresence>
    {
        public void Configure(EntityTypeBuilder<WorshipPresence> builder)
        {
            builder.ToTable("worship_presences", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WorshipServiceId)
                .HasColumnName("worship_service_id")
                .IsRequired();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(x => x.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired();
        }
    }
}