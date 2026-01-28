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

        public int? DefaultBankingInfoId { get; set; }
        public BankingInfo? DefaultBankingInfo { get; set; }

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
        
        // 📱 REDES SOCIAIS E CONTATOS
        public string? Website { get; set; } // Site oficial da igreja
        public string? Email { get; set; } // Email institucional
        public string? InstagramUrl { get; set; } // URL completa do Instagram
        public string? FacebookUrl { get; set; } // URL completa do Facebook
        public string? YoutubeUrl { get; set; } // URL completa do YouTube
        public string? WhatsAppNumber { get; set; } // Número do WhatsApp (formato internacional)
        public string? TwitterUrl { get; set; } // URL completa do Twitter/X
        public string? TikTokUrl { get; set; } // URL completa do TikTok
        
        // 🏛️ CAPACIDADE E INFRAESTRUTURA
        public int? SeatingCapacity { get; set; } // Lotação total de assentos
        public int? StandingCapacity { get; set; } // Capacidade em pé (para eventos especiais)
        public int? ParkingSpaces { get; set; } // Número de vagas de estacionamento
        public bool HasWifi { get; set; } = false; // WiFi disponível
        public string? WifiPassword { get; set; } // Senha do WiFi (opcional, para membros)
        public bool HasCafeteria { get; set; } = false; // Possui cafeteria/lanchonete
        public bool HasBookstore { get; set; } = false; // Possui livraria
        public bool HasNursery { get; set; } = false; // Possui berçário
        public bool HasSoundSystem { get; set; } = false; // Sistema de som profissional
        public bool HasProjector { get; set; } = false; // Projetor/Telão
        public bool HasAirConditioning { get; set; } = false; // Ar condicionado
        public bool HasBaptistery { get; set; } = false; // Batistério
        public string? AdditionalFacilities { get; set; } // JSON array de instalações adicionais
        public string? EquipmentNotes { get; set; } // Notas sobre equipamentos disponíveis
        
        public int? ParentChurchId { get; set; }
        public Church? ParentChurch { get; set; }
        public ICollection<Church> Branches { get; set; } = new List<Church>();

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
        /// Atualiza as redes sociais e contatos da igreja
        /// </summary>
        public void UpdateSocialMedia(
            string? website = null,
            string? email = null,
            string? instagramUrl = null,
            string? facebookUrl = null,
            string? youtubeUrl = null,
            string? whatsAppNumber = null,
            string? twitterUrl = null,
            string? tiktokUrl = null)
        {
            if (website != null) Website = website;
            if (email != null) Email = email;
            if (instagramUrl != null) InstagramUrl = instagramUrl;
            if (facebookUrl != null) FacebookUrl = facebookUrl;
            if (youtubeUrl != null) YoutubeUrl = youtubeUrl;
            if (whatsAppNumber != null) WhatsAppNumber = whatsAppNumber;
            if (twitterUrl != null) TwitterUrl = twitterUrl;
            if (tiktokUrl != null) TikTokUrl = tiktokUrl;
            
            Updated = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Atualiza capacidade e infraestrutura da igreja
        /// </summary>
        public void UpdateCapacityAndInfrastructure(
            int? seatingCapacity = null,
            int? standingCapacity = null,
            int? parkingSpaces = null,
            bool? hasWifi = null,
            string? wifiPassword = null,
            bool? hasCafeteria = null,
            bool? hasBookstore = null,
            bool? hasNursery = null,
            bool? hasSoundSystem = null,
            bool? hasProjector = null,
            bool? hasAirConditioning = null,
            bool? hasBaptistery = null,
            string? additionalFacilities = null,
            string? equipmentNotes = null)
        {
            if (seatingCapacity.HasValue) SeatingCapacity = seatingCapacity.Value;
            if (standingCapacity.HasValue) StandingCapacity = standingCapacity.Value;
            if (parkingSpaces.HasValue) ParkingSpaces = parkingSpaces.Value;
            if (hasWifi.HasValue) HasWifi = hasWifi.Value;
            if (wifiPassword != null) WifiPassword = wifiPassword;
            if (hasCafeteria.HasValue) HasCafeteria = hasCafeteria.Value;
            if (hasBookstore.HasValue) HasBookstore = hasBookstore.Value;
            if (hasNursery.HasValue) HasNursery = hasNursery.Value;
            if (hasSoundSystem.HasValue) HasSoundSystem = hasSoundSystem.Value;
            if (hasProjector.HasValue) HasProjector = hasProjector.Value;
            if (hasAirConditioning.HasValue) HasAirConditioning = hasAirConditioning.Value;
            if (hasBaptistery.HasValue) HasBaptistery = hasBaptistery.Value;
            if (additionalFacilities != null) AdditionalFacilities = additionalFacilities;
            if (equipmentNotes != null) EquipmentNotes = equipmentNotes;
            
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
