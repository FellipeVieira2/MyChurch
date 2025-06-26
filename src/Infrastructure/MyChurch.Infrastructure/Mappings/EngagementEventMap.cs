using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Infrastructure.Mappings
{
    public class EngagementEventMap : IEntityTypeConfiguration<EngagementEvent>
    {
        public void Configure(EntityTypeBuilder<EngagementEvent> builder)
        {
            builder.ToTable("engagement_event", "postgres");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("uuid")
                .IsRequired();

            builder.Property(e => e.MemberId)
                .HasColumnName("member_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(e => e.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(e => e.Points)
                .HasColumnName("points")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(e => e.EventType)
                .HasColumnName("event_type")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(e => e.EventReferenceId)
                .HasColumnName("event_reference_id")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .IsRequired();

            builder.HasIndex(e => new { e.MemberId, e.CreatedAt });
            builder.HasIndex(e => e.ChurchId);

            builder.HasOne(e => e.Member)
                .WithMany()
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
