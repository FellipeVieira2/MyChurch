using System.Data;
using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Document { get; set; }
        public string? Photo { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsBaptized { get; set; }
        public DateTime BaptizedDate { get; set; }
        public bool IsTither { get; set; }
        public int ChurchId { get; set; }
        public Church Church { get; set; }
        public UserRole Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
