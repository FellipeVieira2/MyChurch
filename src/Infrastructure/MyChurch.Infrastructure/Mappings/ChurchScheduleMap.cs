using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class ChurchScheduleMap : IEntityTypeConfiguration<ChurchSchedule>
    {
        public void Configure(EntityTypeBuilder<ChurchSchedule> builder)
        {
            builder.ToTable("church_schedules", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.DayOfWeek)
                .HasColumnName("day_of_week")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.StartTime)
                .HasColumnName("start_time")
                .HasColumnType("time")
                .IsRequired();

            builder.Property(x => x.EndTime)
                .HasColumnName("end_time")
                .HasColumnType("time");

            builder.Property(x => x.ServiceType)
                .HasColumnName("service_type")
                .HasColumnType("varchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(500)")
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .HasColumnType("boolean")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp");

            // Relacionamento com Church
            builder.HasOne(x => x.Church)
                .WithMany(c => c.Schedules)
                .HasForeignKey(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índice para melhorar consultas
            builder.HasIndex(x => new { x.ChurchId, x.DayOfWeek, x.IsActive })
                .HasDatabaseName("IX_church_schedules_church_day_active");
        }
    }
}
