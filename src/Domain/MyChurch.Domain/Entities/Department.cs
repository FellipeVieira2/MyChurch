namespace MyChurch.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public int? BankingInfoId { get; set; }
        public BankingInfo? BankingInfo { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }

        public ICollection<DepartmentMember> Members { get; set; } = new List<DepartmentMember>();
    }
}
