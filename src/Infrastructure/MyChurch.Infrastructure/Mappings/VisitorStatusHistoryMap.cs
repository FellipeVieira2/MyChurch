using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class VisitorStatusHistoryMap : IEntityTypeConfiguration<VisitorStatusHistory>
    {
        public void Configure(EntityTypeBuilder<VisitorStatusHistory> builder)
        {
            builder.ToTable("visitor_status_history", "postgres");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(x => x.VisitorId).HasColumnName("visitor_id").IsRequired();
            builder.Property(x => x.OldStatus).HasColumnName("old_status").IsRequired();
            builder.Property(x => x.NewStatus).HasColumnName("new_status").IsRequired();
            builder.Property(x => x.ChangedByMemberId).HasColumnName("changed_by_member_id");
            builder.Property(x => x.ChangedAt).HasColumnName("changed_at");
            builder.Property(x => x.Note).HasColumnName("note");
            builder.HasOne(x => x.Visitor).WithMany(v => v.StatusHistory).HasForeignKey(x => x.VisitorId);
        }
    }
}
