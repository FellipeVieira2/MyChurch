using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class PlanDto
    {
        public int Id { get; set; }
        public PlanTier Tier { get; set; }
        public string Name { get; set; } // Free, ProSmall, Pro, ProPlus
        public decimal Price { get; set; }

        public int MaxMembers { get; set; }
        public int MaxEvents { get; set; }
        public int MaxStorageGB { get; set; }

        public int MaxAdmins { get; set; }
        public int MaxLeaders { get; set; }
        public int MaxDonationsPerMonth { get; set; }

        public bool ShowsAds { get; set; }
        public bool CanExportCsv { get; set; }
        public bool CanExportPdf { get; set; }
        public bool HasDepartmentReports { get; set; }
        public bool HasAdvancedPermissions { get; set; }
        public bool HasAuditTrail { get; set; }
        public bool HasAutomations { get; set; }

        public bool CanPromoteChurch { get; set; }
        public bool CanPromoteEvents { get; set; }
        public bool HasAdvancedAnalytics { get; set; }
        public bool HasPrioritySupport { get; set; }
        public bool HasVerifiedBadge { get; set; }

        public static PlanDto New(MyChurch.Domain.Entities.Plan plan)
        {
            return new PlanDto
            {
                Id = plan.Id,
                Tier = plan.Tier,
                Name = plan.Name,
                Price = plan.Price,
                MaxMembers = plan.MaxMembers,
                MaxEvents = plan.MaxEvents,
                MaxStorageGB = plan.MaxStorageGB,
                MaxAdmins = plan.MaxAdmins,
                MaxLeaders = plan.MaxLeaders,
                MaxDonationsPerMonth = plan.MaxDonationsPerMonth,
                ShowsAds = plan.ShowsAds,
                CanExportCsv = plan.CanExportCsv,
                CanExportPdf = plan.CanExportPdf,
                HasDepartmentReports = plan.HasDepartmentReports,
                HasAdvancedPermissions = plan.HasAdvancedPermissions,
                HasAuditTrail = plan.HasAuditTrail,
                HasAutomations = plan.HasAutomations,
                CanPromoteChurch = plan.CanPromoteChurch,
                CanPromoteEvents = plan.CanPromoteEvents,
                HasAdvancedAnalytics = plan.HasAdvancedAnalytics,
                HasPrioritySupport = plan.HasPrioritySupport,
                HasVerifiedBadge = plan.HasVerifiedBadge
            };
        }
    }
}
