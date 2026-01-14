using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Plan
    {
        public int Id { get; set; }

        public PlanTier Tier { get; set; }

        public string Name { get; set; } // Free, ProSmall, Pro, ProPlus
        public decimal Price { get; set; }

        public int MaxMembers { get; set; } // Quantidade de membros permitidos
        public int MaxEvents { get; set; } // Quantidade de eventos permitidos
        public int MaxStorageGB { get; set; } // Espaço de armazenamento
        public int Branches { get; set; } // Quantidade de filiais permitidas

        // Limites adicionais (saas por igreja)
        public int MaxAdmins { get; set; }
        public int MaxLeaders { get; set; }
        public int MaxDonationsPerMonth { get; set; }

        // Features
        public bool ShowsAds { get; set; } = true;
        public bool CanExportCsv { get; set; } = false;
        public bool CanExportPdf { get; set; } = false;
        public bool HasDepartmentReports { get; set; } = false;
        public bool HasAdvancedPermissions { get; set; } = false;
        public bool HasAuditTrail { get; set; } = false;
        public bool HasAutomations { get; set; } = false;

        // ✅ já existiam
        public bool CanPromoteChurch { get; set; } = false;
        public bool CanPromoteEvents { get; set; } = false;
        public bool HasAdvancedAnalytics { get; set; } = false;
        public bool HasPrioritySupport { get; set; } = false;
        public bool HasVerifiedBadge { get; set; } = false;

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
