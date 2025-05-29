using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class EventRecurrenceMap : IEntityTypeConfiguration<EventRecurrence>
    {
        public void Configure(EntityTypeBuilder<EventRecurrence> builder)
        {
            builder.ToTable("event_recurrence", "postgres");

            builder.HasKey(r => r.Id);

            builder
                .Property(r => r.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(r => r.EventId)
                .HasColumnName("event_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(r => r.RecurrenceType)
                .HasColumnName("recurrence_type")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(r => r.Frequency)
                .HasColumnName("frequency")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(r => r.RecurrenceEndDate)
                .HasColumnName("recurrence_end_date")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .HasOne(r => r.Event)
                .WithOne(r => r.Recurrence)
                .HasForeignKey<EventRecurrence>(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
