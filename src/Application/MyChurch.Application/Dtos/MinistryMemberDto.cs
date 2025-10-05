using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class MinistryMemberDto
    {
        public int Id { get; set; }
        public int MinistryId { get; set; }
        public string? MinistryName { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public string? MemberPhone { get; set; }
        public string? MemberPhoto { get; set; }
        public string? Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        
        public static MinistryMemberDto FromEntity(MinistryMember ministryMember)
        {
            return new MinistryMemberDto
            {
                Id = ministryMember.Id,
                MinistryId = ministryMember.MinistryId,
                MinistryName = ministryMember.Ministry?.Name,
                MemberId = ministryMember.MemberId,
                MemberName = ministryMember.Member?.Name,
                MemberEmail = ministryMember.Member?.Email,
                MemberPhone = ministryMember.Member?.Phone,
                MemberPhoto = ministryMember.Member?.Photo,
                Role = ministryMember.Role,
                JoinedAt = ministryMember.JoinedAt,
                Notes = ministryMember.Notes,
                IsActive = ministryMember.IsActive
            };
        }
    }
}
