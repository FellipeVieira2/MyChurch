using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Photo { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsBaptized { get; set; }
        public DateTime BaptizedDate { get; set; }
        public bool IsTither { get; set; }
        public int? ChurchId { get; set; }
        public ChurchDto Church { get; set; }
        public UserRole Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public static MemberDto New(Domain.Entities.Member member)
        {
            return new MemberDto
            {
                Id = member.Id,
                Name = member.Name,
                Document = member.Document,
                Email = member.Email,
                Phone = member.Phone,
                BirthDate = member.BirthDate,
                IsBaptized = member.IsBaptized,
                IsTither = member.IsTither,
                Role = member.Role,
                Created = member.Created,
                Updated = member.Updated,
                ChurchId = member.ChurchId,
            };
        }
    }
}
