using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.PlatformAdmin.Queries.GetChurchLeads;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/platform-admin")]
    [Authorize(Roles = "PlatformAdmin")]
    public class PlatformAdminController : BaseController
    {
        [HttpGet("churches")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ChurchLeadDto>))]
        public async Task<IActionResult> GetChurchLeads([FromQuery] GetChurchLeadsQuery query)
        {
            // JwtMemberFilter preencherá UserId/Role a partir do token
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
