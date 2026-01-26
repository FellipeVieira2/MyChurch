using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class DepartmentGeneralLeaderScope
    {
        public int Id { get; set; }

        public int ParentChurchId { get; set; }
        public Church ParentChurch { get; set; } = null!;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public int LeaderMemberId { get; set; }
        public Member LeaderMember { get; set; } = null!;

        public int BranchChurchId { get; set; }
        public Church BranchChurch { get; set; } = null!;

        public bool IsActive { get; set; } = true;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}
