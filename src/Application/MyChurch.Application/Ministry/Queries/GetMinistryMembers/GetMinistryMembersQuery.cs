using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Ministries.Queries.GetMinistryMembers
{
    public class GetMinistryMembersQuery : IRequest<List<MinistryMemberDto>>
    {
        public int MinistryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
