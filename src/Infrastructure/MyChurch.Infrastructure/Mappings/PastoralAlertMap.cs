using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PastoralAlertMap : IEntityTypeConfiguration<PastoralAlert>
    {
        public void Configure(EntityTypeBuilder<PastoralAlert> builder)
        {
            builder.ToTable("pastoral_alerts");

            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.Id).HasColumnName("id");
            builder.Property(pa => pa.ChurchId).HasColumnName("church_id").IsRequired();
            builder.Property(pa => pa.MemberId).HasColumnName("member_id").IsRequired();
            builder.Property(pa => pa.LeaderId).HasColumnName("leader_id");
            builder.Property(pa => pa.Message).HasColumnName("message").IsRequired();
            builder.Property(pa => pa.Source).HasColumnName("source").IsRequired();
            builder.Property(pa => pa.IsRead).HasColumnName("is_read").IsRequired();
            builder.Property(pa => pa.CreatedAt).HasColumnName("created_at").IsRequired();
        }
    }
}
