namespace Mychurch.App.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public MemberDto Member { get; set; } = new MemberDto();
    }

    public class MemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
    }
}
