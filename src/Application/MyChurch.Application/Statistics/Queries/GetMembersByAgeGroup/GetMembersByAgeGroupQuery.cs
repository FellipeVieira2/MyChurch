using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Statistics.Queries.GetMembersByAgeGroup
{
    public class GetMembersByAgeGroupQuery : JwtMemberDto, IRequest<List<MembersByAgeGroupDto>>
    {
        public int? ChurchId { get; set; }
    }
}
