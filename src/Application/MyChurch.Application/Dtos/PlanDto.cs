using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class PlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } // Free, Premium  
        public decimal Price { get; set; }
        public int MaxMembers { get; set; } // Quantidade de membros permitidos  
        public int MaxEvents { get; set; } // Quantidade de eventos permitidos  
        public int MaxStorageGB { get; set; } // Espaço de armazenamento
        
        // 🆕 FEATURES PREMIUM
        public bool ShowsAds { get; set; }
        public bool CanPromoteChurch { get; set; }
        public bool CanPromoteEvents { get; set; }
        public bool HasAdvancedAnalytics { get; set; }
        public bool HasPrioritySupport { get; set; }
        public bool HasVerifiedBadge { get; set; }

        public static PlanDto New(Domain.Entities.Plan plan)
        {
            return new PlanDto
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                MaxMembers = plan.MaxMembers,
                MaxEvents = plan.MaxEvents,
                MaxStorageGB = plan.MaxStorageGB,
                ShowsAds = plan.ShowsAds,
                CanPromoteChurch = plan.CanPromoteChurch,
                CanPromoteEvents = plan.CanPromoteEvents,
                HasAdvancedAnalytics = plan.HasAdvancedAnalytics,
                HasPrioritySupport = plan.HasPrioritySupport,
                HasVerifiedBadge = plan.HasVerifiedBadge
            };
        }
    }
}
