using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class WorshipServiceMap : IEntityTypeConfiguration<WorshipService>
    {
        public void Configure(EntityTypeBuilder<WorshipService> builder)
        {
            builder.ToTable("worship_services", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .IsRequired();

            builder.Property(x => x.Theme)
                .HasColumnName("theme");

            builder.Property(x => x.StartTime)
                .HasColumnName("start_time")
                .IsRequired();

            builder.Property(ws => ws.EventId)
                .HasColumnName("event_id")
                .IsRequired();

            builder.Property(ws => ws.DepartmentId)
                .HasColumnName("department_id");

            builder.HasOne(ws => ws.Event)
                .WithMany(e => e.WorshipServices)
                .HasForeignKey(ws => ws.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ws => ws.Department)
                .WithMany()
                .HasForeignKey(ws => ws.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.EndTime)
                .HasColumnName("end_time");

            builder.Property(x => x.Description)
                .HasColumnName("description");

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasConversion<int>()
                .IsRequired();

            builder.HasMany(x => x.Activities)
                .WithOne(x => x.WorshipService)
                .HasForeignKey(x => x.WorshipServiceId);

            builder.HasMany(x => x.Presences)
                .WithOne()
                .HasForeignKey(x => x.WorshipServiceId);

            builder.HasMany(x => x.Schedule)
                .WithOne(x => x.WorshipService)
                .HasForeignKey(x => x.WorshipServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(ws => ws.DonationWorshipServices)
                .WithOne(dws => dws.WorshipService)
                .HasForeignKey(dws => dws.WorshipServiceId);
        }
    }
}