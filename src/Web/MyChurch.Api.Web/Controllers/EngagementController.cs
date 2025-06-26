using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Engagement.Queries;
using MyChurch.Application.Dtos;
using Mychurch.Common.Utils.Objects;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EngagementController : BaseController
    {
        /// <summary>
        /// Retorna o ranking de engajamento dos membros da igreja
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("ranking")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDto<MemberEngagementRankingDto>))]
        public async Task<IActionResult> GetRanking( [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var query = AuthorizationRequestCreate<GetMemberEngagementRankingQuery>();
            query.PageNumber = pageNumber;
            query.PageSize = pageSize;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retorna o score médio de engajamento da comunidade
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("community-score")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommunityEngagementScoreDto))]
        public async Task<IActionResult> GetCommunityScore()
        {
            var query = AuthorizationRequestCreate<GetCommunityEngagementScoreQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
