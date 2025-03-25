using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PaymentMap : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payment", "postgres");

            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(p => p.SubscriptionId)
                .HasColumnName("subscription_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder
                .Property(p => p.Date)
                .HasColumnName("date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(p => p.PaymentStatus)
                .HasColumnName("payment_status")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder
                .Property(p => p.TransactionId)
                .HasColumnName("transaction_id")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .HasOne(p => p.Subscription)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
