namespace MyChurch.Application.Dtos
{
    public class VisitorJwtDto
    {
        public int VisitorId { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = "Visitor";
    }
}
