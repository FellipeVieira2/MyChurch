namespace MyChurch.Domain.Entities
{
    public class ChildPickupAuthorization
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public Child Child { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public int? CreatedByMemberId { get; set; }
        public Member? CreatedByMember { get; set; }
    }
}
