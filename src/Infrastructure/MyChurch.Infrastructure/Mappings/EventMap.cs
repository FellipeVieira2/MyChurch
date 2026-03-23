using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class EventMap : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("event", "postgres");

            builder.HasKey(e => e.Id);

            builder
                .Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .IsRequired();

            builder
                .Property(e => e.Date)
                .HasColumnName("date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(e => e.FinishDate)
                .HasColumnName("finish_date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(e => e.Location)
                .HasColumnName("location")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder
                .Property(e => e.RequiresParticipantList)
                .HasColumnName("requires_participant_list")
                .HasColumnType("boolean")
                .IsRequired();

            builder
                .Property(e => e.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(e => e.EventType)
                .HasColumnName("event_type")
                .HasColumnType("integer")
                .IsRequired();

            builder
                .Property(e => e.DepartmentId)
                .HasColumnName("department_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .Property<int?>("WorshipServiceId")
                .HasColumnName("worship_service_id")
                .HasColumnType("integer")
                .IsRequired(false);

            builder
                .HasOne(e => e.Church)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder
                .HasMany(e => e.WorshipServices)
                .WithOne(ws => ws.Event)
                .HasForeignKey(ws => ws.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(e => e.Participants)
                .WithMany(m => m.Events)
                .UsingEntity<Dictionary<string, object>>(
                    "EventParticipants",
                    j => j
                        .HasOne<Member>()
                        .WithMany()
                        .HasForeignKey("member_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Event>()
                        .WithMany()
                        .HasForeignKey("event_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.ToTable("event_participants");
                        j.HasKey("event_id", "member_id");
                    });

            builder
                .HasMany(e => e.DiaconateScaleMembers)
                .WithOne(x => x.Event)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(e => e.KidsScaleMembers)
                .WithOne(x => x.Event)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(e => e.Notifications)
                .WithOne(n => n.Event)
                .HasForeignKey(n => n.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(r => r.Recurrence)
                .WithOne(r => r.Event)
                .HasForeignKey<EventRecurrence>(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}