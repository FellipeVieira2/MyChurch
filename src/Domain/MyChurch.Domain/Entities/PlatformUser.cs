using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class PlatformUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public UserRole Role { get; set; } = UserRole.PlatformAdmin;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}
