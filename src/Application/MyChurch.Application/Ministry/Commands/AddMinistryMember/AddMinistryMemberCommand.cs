using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Ministries.Commands.AddMinistryMember
{
    public class AddMinistryMemberCommand : IRequest<MinistryMemberDto>
    {
        public int MinistryId { get; set; }
        public int MemberId { get; set; }
        public string? Role { get; set; }
        public string? Notes { get; set; }
    }
}
