using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class TransferHistoryMap : IEntityTypeConfiguration<TransferHistory>
    {
        public void Configure(EntityTypeBuilder<TransferHistory> builder)
        {
            builder.ToTable("transfer_history", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(x => x.ChurchId).HasColumnName("church_id").IsRequired();

            builder.Property(x => x.BankingInfoId).HasColumnName("banking_info_id");

            builder.Property(x => x.Amount).HasColumnName("amount").IsRequired();
            builder.Property(x => x.RequestedAt).HasColumnName("requested_at").IsRequired();
            builder.Property(x => x.ScheduledFor).HasColumnName("scheduled_for");
            builder.Property(x => x.CompletedAt).HasColumnName("completed_at");
            builder.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
            builder.Property(x => x.FailureReason).HasColumnName("failure_reason").HasMaxLength(1000);

            builder.HasOne(x => x.Church)
                .WithMany()
                .HasForeignKey(x => x.ChurchId);

            builder.HasOne(x => x.BankingInfo)
                .WithMany()
                .HasForeignKey(x => x.BankingInfoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}