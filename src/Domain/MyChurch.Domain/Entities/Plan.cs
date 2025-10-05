namespace MyChurch.Domain.Entities
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; } // Free, Premium  
        public decimal Price { get; set; }
        public int MaxMembers { get; set; } // Quantidade de membros permitidos  
        public int MaxEvents { get; set; } // Quantidade de eventos permitidos  
        public int MaxStorageGB { get; set; } // Espaço de armazenamento  
        public int Branches { get; set; } // Quantidade de filiais permitidas
        
        // 🆕 FEATURES PREMIUM
        public bool ShowsAds { get; set; } = true; // Se mostra ads internos (Free = true, Premium = false)
        public bool CanPromoteChurch { get; set; } = false; // Se pode promover igreja
        public bool CanPromoteEvents { get; set; } = false; // Se pode promover eventos
        public bool HasAdvancedAnalytics { get; set; } = false; // Analytics avançados
        public bool HasPrioritySupport { get; set; } = false; // Suporte prioritário
        public bool HasVerifiedBadge { get; set; } = false; // Badge de verificação
        
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
