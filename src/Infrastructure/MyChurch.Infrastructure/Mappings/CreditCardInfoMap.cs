using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class CreditCardInfoMap : IEntityTypeConfiguration<CreditCardInfo>
    {
        public void Configure(EntityTypeBuilder<CreditCardInfo> builder)
        {
            builder.ToTable("credit_card_info", "postgres");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(c => c.MemberId)
                .HasColumnName("member_id")
                .IsRequired();

            builder.Property(c => c.Last4Digits)
                .HasColumnName("last4digits")
                .HasColumnType("varchar(4)")
                .IsRequired();

            builder.Property(c => c.CardBrand)
                .HasColumnName("card_brand")
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(c => c.CardHash)
                .HasColumnName("card_hash")
                .HasColumnType("varchar(500)")
                .IsRequired();

            builder.Property(c => c.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.HasOne(c => c.Member)
                .WithMany(c => c.CreditCardInfos)
                .HasForeignKey(c => c.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}