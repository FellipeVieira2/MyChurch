namespace MyChurch.Application.Dtos
{
    public class JwtMemberDto
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}