using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyChurch.Domain.Entities;

namespace MyChurch.Infrastructure.Mappings
{
    public class PlanMap : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTable("plan", "postgres");

            builder.HasKey(p => p.Id);

            builder
                .Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("int")
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder
                .Property(p => p.Tier)
                .HasColumnName("tier")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder
                .HasIndex(p => p.Tier)
                .IsUnique();

            builder
                .Property(p => p.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(10,2)")
                .IsRequired();

            builder
                .Property(p => p.MaxMembers)
                .HasColumnName("max_members")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.MaxEvents)
                .HasColumnName("max_events")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.Branches)
                .HasColumnName("branches")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.MaxStorageGB)
                .HasColumnName("max_storage_gb")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.MaxAdmins)
                .HasColumnName("max_admins")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.MaxLeaders)
                .HasColumnName("max_leaders")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.MaxDonationsPerMonth)
                .HasColumnName("max_donations_per_month")
                .HasColumnType("int")
                .IsRequired();

            builder
                .Property(p => p.ShowsAds)
                .HasColumnName("shows_ads")
                .HasColumnType("boolean")
                .HasDefaultValue(true)
                .IsRequired();

            builder
                .Property(p => p.CanExportCsv)
                .HasColumnName("can_export_csv")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.CanExportPdf)
                .HasColumnName("can_export_pdf")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.HasDepartmentReports)
                .HasColumnName("has_department_reports")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.HasAdvancedPermissions)
                .HasColumnName("has_advanced_permissions")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.HasAuditTrail)
                .HasColumnName("has_audit_trail")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.HasAutomations)
                .HasColumnName("has_automations")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.CanPromoteChurch)
                .HasColumnName("can_promote_church")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.CanPromoteEvents)
                .HasColumnName("can_promote_events")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.HasAdvancedAnalytics)
                .HasColumnName("has_advanced_analytics")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.HasPrioritySupport)
                .HasColumnName("has_priority_support")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.HasVerifiedBadge)
                .HasColumnName("has_verified_badge")
                .HasColumnType("boolean")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .Property(p => p.Created)
                .HasColumnName("created")
                .HasColumnType("timestamp")
                .IsRequired();

            builder
                .Property(p => p.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp")
                .IsRequired(false);
        }
    }
}
