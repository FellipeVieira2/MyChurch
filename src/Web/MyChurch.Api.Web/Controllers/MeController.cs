using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Queries.GetMyChurches;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class MeController : BaseController
    {
        [HttpGet("churches")]
        public async Task<IActionResult> GetMyChurches()
        {
            var q = AuthorizationRequestCreate<GetMyChurchesQuery>();
            var result = await Mediator.Send(q);
            return Ok(result);
        }
    }
}
