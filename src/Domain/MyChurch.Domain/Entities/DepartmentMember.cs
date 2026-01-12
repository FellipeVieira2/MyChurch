using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class DepartmentMember
    {
        public int Id { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public DepartmentMemberRole Role { get; set; } = DepartmentMemberRole.Member;

        public bool IsActive { get; set; } = true;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
