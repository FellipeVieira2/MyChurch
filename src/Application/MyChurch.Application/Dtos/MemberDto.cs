using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MemberDocumentDto> Document { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Photo { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsBaptized { get; set; }
        public DateTime? BaptizedDate { get; set; }
        public bool IsTither { get; set; }
        public int? ChurchId { get; set; }
        public ChurchDto Church { get; set; }
        public UserRole Role { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? MaritalStatus { get; set; }
        public DateTime? MemberSince { get; set; }
        public string? Ministry { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public AddressDto Address { get; set; }

        public static MemberDto New(Domain.Entities.Member member)
        {
            return new MemberDto
            {
                Id = member.Id,
                Name = member.Name,
                Document = [.. member.Documents.Select(MemberDocumentDto.New)],
                Email = member.Email,
                Phone = member.Phone,
                Photo = member.Photo,
                BirthDate = member.BirthDate,
                IsBaptized = member.IsBaptized,
                BaptizedDate = member.BaptizedDate,
                IsTither = member.IsTither,
                ChurchId = member.ChurchId,
                Role = member.Role,
                Created = member.Created,
                Updated = member.Updated,
                MaritalStatus = member.MaritalStatus?.ToString(),
                MemberSince = member.MemberSince,
                Ministry = member.Ministry?.ToString(),
                IsActive = member.IsActive,
                Notes = member.Notes,
                Address =member.Address is not null ? AddressDto.New(member.Address) : null
            };
        }
    }
}
