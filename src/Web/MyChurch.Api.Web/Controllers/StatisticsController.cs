using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.Statistics.Queries.GetChurchStatistics;
using MyChurch.Application.Statistics.Queries.GetMembersByAgeGroup;
using MyChurch.Application.Statistics.Queries.GetMembersByMinistry;
using MyChurch.Application.Statistics.Queries.GetMonthlyFinancial;
using MyChurch.Application.Statistics.Queries.GetMonthlyMemberGrowth;
using MyChurch.Application.Statistics.Queries.GetTopEngagedMembers;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : BaseController
    {
        /// <summary>
        /// Obtém estatísticas gerais da igreja
        /// </summary>
        /// <response code="200">Sucesso: Estatísticas retornadas</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChurchStatisticsDto))]
        public async Task<IActionResult> GetDashboardStatistics([FromQuery] int? churchId = null)
        {
            var query = AuthorizationRequestCreate<GetChurchStatisticsQuery>();
            query.ChurchId = churchId;
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtém distribuição de membros por faixa etária
        /// </summary>
        /// <response code="200">Sucesso: Distribuição retornada</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("members/age-groups")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MembersByAgeGroupDto>))]
        public async Task<IActionResult> GetMembersByAgeGroup([FromQuery] int? churchId = null)
        {
            var query = AuthorizationRequestCreate<GetMembersByAgeGroupQuery>();
            query.ChurchId = churchId;
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtém distribuição de membros por ministério
        /// </summary>
        /// <response code="200">Sucesso: Distribuição retornada</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("members/ministries")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MembersByMinistryDto>))]
        public async Task<IActionResult> GetMembersByMinistry([FromQuery] int? churchId = null)
        {
            var query = AuthorizationRequestCreate<GetMembersByMinistryQuery>();
            query.ChurchId = churchId;
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtém evolução mensal de membros
        /// </summary>
        /// <response code="200">Sucesso: Evolução retornada</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("members/monthly-growth")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MonthlyMemberGrowthDto>))]
        public async Task<IActionResult> GetMonthlyMemberGrowth(
            [FromQuery] int? churchId = null,
            [FromQuery] int months = 12)
        {
            var query = AuthorizationRequestCreate<GetMonthlyMemberGrowthQuery>();
            query.ChurchId = churchId;
            query.Months = months;
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtém evolução financeira mensal
        /// </summary>
        /// <response code="200">Sucesso: Evolução retornada</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("financial/monthly")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MonthlyFinancialDto>))]
        public async Task<IActionResult> GetMonthlyFinancial(
            [FromQuery] int? churchId = null,
            [FromQuery] int months = 12)
        {
            var query = AuthorizationRequestCreate<GetMonthlyFinancialQuery>();
            query.ChurchId = churchId;
            query.Months = months;
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtém top membros mais engajados
        /// </summary>
        /// <response code="200">Sucesso: Top membros retornados</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("members/top-engaged")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TopEngagedMembersDto>))]
        public async Task<IActionResult> GetTopEngagedMembers(
            [FromQuery] int? churchId = null,
            [FromQuery] int top = 10)
        {
            var query = AuthorizationRequestCreate<GetTopEngagedMembersQuery>();
            query.ChurchId = churchId;
            query.Top = top;
            
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
