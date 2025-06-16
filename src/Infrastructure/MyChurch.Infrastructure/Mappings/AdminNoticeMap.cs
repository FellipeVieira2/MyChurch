using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class AdminNoticeMap : IEntityTypeConfiguration<AdminNotice>
    {
        public void Configure(EntityTypeBuilder<AdminNotice> builder)
        {
            builder.ToTable("AdminNotices");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Message).IsRequired();
            builder.Property(x => x.ImageUrl).HasMaxLength(500);
            builder.Property(x => x.Created).IsRequired();
            builder.HasOne(x => x.Church)
                .WithMany()
                .HasForeignKey(x => x.ChurchId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Member)
                .WithMany()
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
