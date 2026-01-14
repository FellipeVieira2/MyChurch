namespace MyChurch.Application.Dtos
{
    public class PlatformUserJwtDto
    {
        public int PlatformUserId { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = "PlatformAdmin";
    }
}
