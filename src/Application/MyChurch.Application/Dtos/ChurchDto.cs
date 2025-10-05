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
        
        // 🕐 HORÁRIOS DE CULTOS
        public List<ChurchScheduleDto>? Schedules { get; set; }

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
                Schedules = church.Schedules?.Select(ChurchScheduleDto.New).ToList()
            };
        }
    }
}
