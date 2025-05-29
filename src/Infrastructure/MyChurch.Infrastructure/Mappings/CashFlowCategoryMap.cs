using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Configurations
{
    public class CashFlowCategoryMap : IEntityTypeConfiguration<CashFlowCategory>
    {
        public void Configure(EntityTypeBuilder<CashFlowCategory> builder)
        {
            builder.ToTable("cash_flow_categories", "postgres");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description");

            builder.Property(x => x.ChurchId)
                .HasColumnName("church_id")
                .IsRequired();

            builder.HasOne(x => x.Church)
                .WithMany(x => x.CashFlowCategories)
                .HasForeignKey(x => x.ChurchId);
        }
    }
}
