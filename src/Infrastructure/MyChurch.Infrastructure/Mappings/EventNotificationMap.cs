using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class EventNotificationMap : IEntityTypeConfiguration<EventNotification>
    {
        public void Configure(EntityTypeBuilder<EventNotification> builder)
        {
            builder.ToTable("event_notification", "postgres");

            builder.HasKey(n => n.Id);

            builder
                .Property(n => n.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(n => n.EventId)
                .HasColumnName("event_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(n => n.SentAt)
                .HasColumnName("sent_at")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(n => n.Message)
                .HasColumnName("message")
                .HasColumnType("text")
                .IsRequired();

            builder
                .HasOne(n => n.Event)
                .WithMany(e => e.Notifications) // Relacionamento 1:N
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
