namespace MyChurch.Application.Dtos
{
    public class ChildPickupAuthorizationInputDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class ChildPickupAuthorizationDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class KidsCheckInSummaryDto
    {
        public int Id { get; set; }
        public string EnvironmentName { get; set; } = string.Empty;
        public DateTime CheckedInAt { get; set; }
        public DateTime PickupTokenExpiresAt { get; set; }
    }

    public class KidsChildDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<ChildPickupAuthorizationDto> AuthorizedPickups { get; set; } = [];
        public KidsCheckInSummaryDto? ActiveCheckIn { get; set; }
    }

    public class KidsCheckInDto
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = string.Empty;
        public DateTime CheckedInAt { get; set; }
        public DateTime PickupTokenExpiresAt { get; set; }
        public string PickupToken { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class KidsPickupValidationDto
    {
        public int CheckInId { get; set; }
        public int ChildId { get; set; }
        public string ChildName { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = string.Empty;
        public DateTime CheckedInAt { get; set; }
        public DateTime PickupTokenExpiresAt { get; set; }
        public List<ChildPickupAuthorizationDto> AuthorizedPickups { get; set; } = [];
    }
}
