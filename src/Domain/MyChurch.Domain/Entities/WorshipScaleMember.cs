namespace MyChurch.Domain.Entities
{
    public class WorshipScaleMember
    {
        public int Id { get; set; }
        public int WorshipServiceId { get; set; }
        public WorshipService WorshipService { get; set; } = null!;

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public string RoleName { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? Notes { get; set; }
    }
}
