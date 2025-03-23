using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PasswordSalt { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsBaptized { get; set; }
        public bool IsTither { get; set; }
        public int ChurchId { get; set; }
        public ChurchDto Church { get; set; }
        public UserRole Role { get; set; }
    }
}
