using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateChurchWithAdminMember;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/church")]
    public class ChurchWithAdminController : BaseController
    {
        /// <summary>
        /// Cria uma igreja e cria também o membro Admin inicial.
        /// </summary>
        [HttpPost("withadmin")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateWithAdmin([FromBody] CreateChurchWithAdminMemberCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
