using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Statistics.Queries.GetMembersByMinistry
{
    public class GetMembersByMinistryQuery : JwtMemberDto, IRequest<List<MembersByMinistryDto>>
    {
        public int? ChurchId { get; set; }
    }
}
