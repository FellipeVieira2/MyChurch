using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class BankingInfoMap : IEntityTypeConfiguration<BankingInfo>
    {
        public void Configure(EntityTypeBuilder<BankingInfo> builder)
        {
            builder.ToTable("banking_info", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .IsRequired();

            builder.Property(x => x.BankName)
                .HasColumnName("bank_name")
                .HasMaxLength(100);

            builder.Property(x => x.BankCode)
                .HasColumnName("bank_code")
                .HasMaxLength(10);
            builder
                .Property(x => x.AccountDigit)
                .HasColumnName("account_digit")
                .HasMaxLength(10);

            builder.Property(x => x.Agency)
                .HasColumnName("agency")
                .HasMaxLength(20);

            builder.Property(x => x.Account)
                .HasColumnName("account")
                .HasMaxLength(30);

            builder.Property(x => x.AccountType)
                .HasColumnName("account_type")
                .HasMaxLength(30);

            builder.Property(x => x.HolderName)
                .HasColumnName("holder_name")
                .HasMaxLength(100);

            builder.Property(x => x.HolderDocument)
                .HasColumnName("holder_document")
                .HasMaxLength(30);

            builder.Property(x => x.PixKey)
                .HasColumnName("pix_key")
                .HasMaxLength(100);

            builder.Property(x => x.PixKeyType)
                .HasColumnName("pix_key_type")
                .HasMaxLength(30);

            builder.Property(x => x.Created)
                .HasColumnName("created")
                .IsRequired();

            builder.Property(x => x.Updated)
                .HasColumnName("updated");

            builder.HasOne(x => x.Church)
                .WithMany()
                .HasForeignKey(x => x.ChurchId);
        }
    }
}