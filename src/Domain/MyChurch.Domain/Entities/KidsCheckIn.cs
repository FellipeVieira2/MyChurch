namespace MyChurch.Domain.Entities
{
    public class KidsCheckIn
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public Child Child { get; set; } = null!;
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;
        public int CheckedInByMemberId { get; set; }
        public Member CheckedInByMember { get; set; } = null!;
        public DateTime CheckedInAt { get; set; }
        public string EnvironmentName { get; set; } = string.Empty;
        public string PickupToken { get; set; } = string.Empty;
        public DateTime PickupTokenExpiresAt { get; set; }
        public string? Notes { get; set; }
        public DateTime? CheckedOutAt { get; set; }
        public int? CheckedOutByMemberId { get; set; }
        public Member? CheckedOutByMember { get; set; }
        public int? AuthorizedPickupId { get; set; }
        public ChildPickupAuthorization? AuthorizedPickup { get; set; }
    }
}
