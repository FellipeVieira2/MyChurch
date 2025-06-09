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
                .Property(p => p.SubscriptionId)
                .HasColumnName("subscription_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .Property(p => p.DonationId)
                .HasColumnName("donation_id")
                .HasColumnType("int")
                .IsRequired(false);

            builder
                .Property(p => p.BillingType)
                .HasColumnName("billing_type")
                .HasColumnType("varchar(30)")
                .IsRequired(false);

            builder
                .HasOne(p => p.Subscription)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.Donation)
                .WithMany(d => d.Payments)
                .HasForeignKey(p => p.DonationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}