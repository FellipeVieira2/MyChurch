using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class VisitorMap : IEntityTypeConfiguration<Visitor>
    {
        public void Configure(EntityTypeBuilder<Visitor> builder)
        {
            builder.ToTable("visitor", "postgres");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.Email).HasColumnName("email");
            builder.Property(x => x.Phone).HasColumnName("phone");
            builder.Property(x => x.AsaasCustomerId).HasColumnName("asaas_customer_id");
            builder.Property(x => x.CreatedAt).HasColumnName("created_at");
            // Novos campos mapeados explicitamente para coincidir com a migration existente
            builder.Property(x => x.Score).HasColumnName("score");
            builder.Property(x => x.Status).HasColumnName("status");
            builder.Property(x => x.NeedsFollowUp).HasColumnName("needs_follow_up");
            builder.Property(x => x.LastVisitAt).HasColumnName("last_visit_at");
        }
    }
}
