using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Statistics.Queries.GetTopEngagedMembers
{
    public class GetTopEngagedMembersQuery : JwtMemberDto, IRequest<List<TopEngagedMembersDto>>
    {
        public int? ChurchId { get; set; }
        
        /// <summary>
        /// Quantidade de membros para retornar (padrão: 10)
        /// </summary>
        public int Top { get; set; } = 10;
    }
}
