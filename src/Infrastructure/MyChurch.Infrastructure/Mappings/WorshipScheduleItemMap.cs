using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class WorshipScheduleItemMap : IEntityTypeConfiguration<WorshipScheduleItem>
    {
        public void Configure(EntityTypeBuilder<WorshipScheduleItem> builder)
        {
            builder.ToTable("worship_schedule_items", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.WorshipServiceId)
                .HasColumnName("worship_service_id")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(x => x.Order)
                .HasColumnName("order")
                .IsRequired();

            builder.HasOne(x => x.WorshipService)
                .WithMany(ws => ws.Schedule)
                .HasForeignKey(x => x.WorshipServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}