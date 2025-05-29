using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class CashFlowEntryConfiguration : IEntityTypeConfiguration<CashFlowEntry>
    {
        public void Configure(EntityTypeBuilder<CashFlowEntry> builder)
        {
            builder.ToTable("cash_flow_entries", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .IsRequired();

            builder.Property(x => x.Date)
                .HasColumnName("date")
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description");

            builder.Property(x => x.Type)
                .HasColumnName("type")
                .IsRequired();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .IsRequired();

            builder.Property(x => x.MemberId)
                .HasColumnName("member_id");

            builder.Property(x => x.CategoryId)
                .HasColumnName("category_id")
                .IsRequired();

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("updated");

            builder.HasOne(x => x.Church)
                .WithMany(x => x.CashFlowEntries)
                .HasForeignKey(x => x.ChurchId);

            builder.HasOne(x => x.Member)
                .WithMany(x => x.CashFlowEntries)
                .HasForeignKey(x => x.MemberId);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.CashFlowEntries)
                .HasForeignKey(x => x.CategoryId);
        }
    }
}
