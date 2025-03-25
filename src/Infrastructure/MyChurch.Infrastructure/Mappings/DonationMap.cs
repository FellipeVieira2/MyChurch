using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class DonationMap : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.ToTable("donation", "postgres");

            builder.HasKey(d => d.Id);

            builder
                .Property(d => d.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(d => d.MemberId)
                .HasColumnName("member_id")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(d => d.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder
                .Property(d => d.Date)
                .HasColumnName("date")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .HasOne(d => d.Member)
                .WithMany(m => m.Donations)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
