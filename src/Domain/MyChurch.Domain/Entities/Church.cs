namespace MyChurch.Domain.Entities
{
    public class Church
    {
        public Church()
        {
        }
        public Church(string name, string phone, Address address, string description)
        {
            Name = name;
            Description = description;
            Phone = phone;
            Address = address;
            Created = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? LogoFileName { get; set; } 
        public string? Description { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public decimal PlatformFee { get; set; } = 0.05m; // valor padrão 5%
        public string Phone { get; set; } 
        public ICollection<Member>? Members { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? AsaasCustomerId { get; set; }
        public string? Document { get; set; }
        public ICollection<Event> Events { get; set; }
        public List<Asset> Assets { get; set; }
        public ICollection<CashFlowCategory> CashFlowCategories { get; set; }
        public ICollection<CashFlowEntry> CashFlowEntries { get; set; }
        public string? OnboardingQrCode { get; set; }
        
        // 🗺️ GEOLOCALIZAÇÃO - Para busca por proximidade
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        // 🏛️ INFORMAÇÕES DE DESCOBERTA
        public string? Denomination { get; set; } // Ex: Batista, Assembleia de Deus, Adventista, Católica
        public string? CoverPhoto { get; set; } // Foto de capa para busca
        
        // ⭐ MÉTRICAS DE AVALIAÇÃO
        public double? AverageRating { get; set; } // Média de avaliações (0-5)
        public int TotalReviews { get; set; } = 0; // Total de avaliações
        public int TotalVisits { get; set; } = 0; // Total de visitas registradas
        
        // 🎯 CARACTERÍSTICAS DA IGREJA (para filtros)
        public bool HasParking { get; set; } = false;
        public bool IsAccessible { get; set; } = false; // Acessível para cadeirantes
        public bool HasLiveStream { get; set; } = false;
        public bool HasChildMinistry { get; set; } = false;
        public string? Languages { get; set; } // JSON array: ["Português", "Inglês", "Espanhol"]
        
        // ✅ VERIFICAÇÃO
        public bool IsVerified { get; set; } = false; // Igreja verificada pela plataforma
        public DateTime? VerifiedAt { get; set; }
        
        public Subscription Subscription { get; set; }
        
        // 🕐 HORÁRIOS DE CULTOS
        public ICollection<ChurchSchedule> Schedules { get; set; }
        
        public void Update(string? name, string? phone)
        {
            Name = name ?? Name;
            Phone = phone ?? Phone;
            Updated = DateTime.UtcNow;
        }

        public void UpdateLogo(string logoFileName)
        {
            LogoFileName = logoFileName;
            Updated = DateTime.UtcNow;
        }
        
        public void UpdateLocation(double? latitude, double? longitude)
        {
            if (latitude.HasValue && (latitude < -90 || latitude > 90))
                throw new ArgumentException("Latitude deve estar entre -90 e 90", nameof(latitude));
            
            if (longitude.HasValue && (longitude < -180 || longitude > 180))
                throw new ArgumentException("Longitude deve estar entre -180 e 180", nameof(longitude));
            
            Latitude = latitude;
            Longitude = longitude;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Atualiza as métricas de avaliação
        /// </summary>
        public void UpdateRatingMetrics(double averageRating, int totalReviews)
        {
            AverageRating = averageRating;
            TotalReviews = totalReviews;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Incrementa contador de visitas
        /// </summary>
        public void IncrementVisitCount()
        {
            TotalVisits++;
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Marca igreja como verificada
        /// </summary>
        public void MarkAsVerified()
        {
            IsVerified = true;
            VerifiedAt = DateTime.UtcNow;
            Updated = DateTime.UtcNow;
        }
    }
}
