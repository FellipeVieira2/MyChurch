namespace MyChurch.Application.Dtos
{
    public class ChurchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public AddressDto Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public ICollection<MemberDto> Members { get; set; } = new List<MemberDto>();

        // Assinatura da igreja  
        public SubscriptionDto Subscription { get; set; }
        
        // Banking information (only visible to admin users)
        public BankingInfoDto BankingInfo { get; set; }

        // Nova lista (suporta múltiplas contas)
        public List<BankingInfoDto>? BankingInfos { get; set; }

        // Exibe apenas para admin
        public string? OnboardingQrCode { get; set; }
        
        // 🗺️ GEOLOCALIZAÇÃO
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        // 🏛️ DESCOBERTA
        public string? Denomination { get; set; }
        public string? CoverPhoto { get; set; }
        
        // ⭐ MÉTRICAS
        public double? AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int TotalVisits { get; set; }
        
        // 🎯 CARACTERÍSTICAS
        public bool HasParking { get; set; }
        public bool IsAccessible { get; set; }
        public bool HasLiveStream { get; set; }
        public bool HasChildMinistry { get; set; }
        public List<string>? Languages { get; set; }
        
        // ✅ VERIFICAÇÃO
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        
        // 📱 REDES SOCIAIS E CONTATOS
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? InstagramUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? YoutubeUrl { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? TwitterUrl { get; set; }
        public string? TikTokUrl { get; set; }
        
        // 🏛️ CAPACIDADE E INFRAESTRUTURA
        public int? SeatingCapacity { get; set; }
        public int? StandingCapacity { get; set; }
        public int? ParkingSpaces { get; set; }
        public bool HasWifi { get; set; }
        public string? WifiPassword { get; set; } // Apenas para membros autenticados
        public bool HasCafeteria { get; set; }
        public bool HasBookstore { get; set; }
        public bool HasNursery { get; set; }
        public bool HasSoundSystem { get; set; }
        public bool HasProjector { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasBaptistery { get; set; }
        public List<string>? AdditionalFacilities { get; set; }
        public string? EquipmentNotes { get; set; }
        
        // 🕐 HORÁRIOS DE CULTOS
        public List<ChurchScheduleDto>? Schedules { get; set; }

        public int? ParentChurchId { get; set; }
        public bool IsBranch => ParentChurchId.HasValue;

        public int BranchesCount { get; set; }
        public int AllowedBranches { get; set; }

        public static ChurchDto New(Domain.Entities.Church church)
        {
            return new ChurchDto
            {
                Id = church.Id,
                Name = church.Name,
                Address = AddressDto.New(church.Address),
                Phone = church.Phone,
                Members = church.Members?.Select(MemberDto.New).ToList() ?? new List<MemberDto>(),
                Subscription = church.Subscription != null ? SubscriptionDto.New(church.Subscription) : null,
                Description = church.Description,
                Logo = church.LogoFileName,
                OnboardingQrCode = church.OnboardingQrCode,
                Latitude = church.Latitude,
                Longitude = church.Longitude,
                Denomination = church.Denomination,
                CoverPhoto = church.CoverPhoto,
                AverageRating = church.AverageRating,
                TotalReviews = church.TotalReviews,
                TotalVisits = church.TotalVisits,
                HasParking = church.HasParking,
                IsAccessible = church.IsAccessible,
                HasLiveStream = church.HasLiveStream,
                HasChildMinistry = church.HasChildMinistry,
                Languages = string.IsNullOrEmpty(church.Languages) 
                    ? null 
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(church.Languages),
                IsVerified = church.IsVerified,
                VerifiedAt = church.VerifiedAt,
                Website = church.Website,
                Email = church.Email,
                InstagramUrl = church.InstagramUrl,
                FacebookUrl = church.FacebookUrl,
                YoutubeUrl = church.YoutubeUrl,
                WhatsAppNumber = church.WhatsAppNumber,
                TwitterUrl = church.TwitterUrl,
                TikTokUrl = church.TikTokUrl,
                SeatingCapacity = church.SeatingCapacity,
                StandingCapacity = church.StandingCapacity,
                ParkingSpaces = church.ParkingSpaces,
                HasWifi = church.HasWifi,
                WifiPassword = church.WifiPassword,
                HasCafeteria = church.HasCafeteria,
                HasBookstore = church.HasBookstore,
                HasNursery = church.HasNursery,
                HasSoundSystem = church.HasSoundSystem,
                HasProjector = church.HasProjector,
                HasAirConditioning = church.HasAirConditioning,
                HasBaptistery = church.HasBaptistery,
                AdditionalFacilities = string.IsNullOrEmpty(church.AdditionalFacilities)
                    ? null
                    : System.Text.Json.JsonSerializer.Deserialize<List<string>>(church.AdditionalFacilities),
                EquipmentNotes = church.EquipmentNotes,
                Schedules = church.Schedules?.Select(ChurchScheduleDto.New).ToList(),
                ParentChurchId = church.ParentChurchId,
                BranchesCount = church.Branches?.Count ?? 0,
                AllowedBranches = church.Subscription?.Plan?.Branches ?? 0,
            };
        }
    }
}
