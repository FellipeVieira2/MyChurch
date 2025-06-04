namespace MyChurch.Application.Dtos
{
    public class LoginDto
    {
        public string Token { get; set; }
        public string Role { get; set; }

        public MemberDto Member { get; set; }
    }
}