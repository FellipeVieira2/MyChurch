using MediatR;
using MyChurch.Application.Dtos;

namespace MyChurch.Application.Statistics.Queries.GetMonthlyMemberGrowth
{
    public class GetMonthlyMemberGrowthQuery : JwtMemberDto, IRequest<List<MonthlyMemberGrowthDto>>
    {
        public int? ChurchId { get; set; }
        
        /// <summary>
        /// Número de meses para retornar (padrão: 12)
        /// </summary>
        public int Months { get; set; } = 12;
    }
}
