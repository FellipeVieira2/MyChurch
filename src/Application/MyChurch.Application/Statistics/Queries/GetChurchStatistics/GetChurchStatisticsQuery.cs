using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Statistics.Queries.GetChurchStatistics
{
    public class GetChurchStatisticsQuery : JwtMemberDto, IRequest<ChurchStatisticsDto>
    {
        /// <summary>
        /// ID da igreja (opcional, pega da autenticação se não fornecido)
        /// </summary>
        public int? ChurchId { get; set; }
    }
}
