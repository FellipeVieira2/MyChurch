using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class SubscriptionMap : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("subscription", "postgres");

            builder.HasKey(s => s.Id);

            builder
                .Property(s => s.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(s => s.ChurchId)
                .HasColumnName("church_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(s => s.PlanId)
                .HasColumnName("plan_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(s => s.StartDate)
                .HasColumnName("start_date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(s => s.EndDate)
                .HasColumnName("end_date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(s => s.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(s => s.ExternalReference)
                .HasColumnName("external_reference")
                .HasColumnType("varchar(100)")
                .IsRequired(false);

            builder
                .Property(s => s.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);

            builder
                .HasOne(s => s.Church)
                .WithOne(c => c.Subscription)
                .HasForeignKey<Subscription>(s => s.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(s => s.Plan)
                .WithMany()
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(s => s.Payments)
                .WithOne(p => p.Subscription)
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
