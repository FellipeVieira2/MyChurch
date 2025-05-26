using System.Text.Json.Serialization;

namespace MyChurch.Application.Dtos
{
    public class JwtMemberDto
    {
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public string? Email { get; set; }
        [JsonIgnore]
        public string? Role { get; set; }
    }
}