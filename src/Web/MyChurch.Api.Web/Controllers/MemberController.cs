using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateChurchCommand;

namespace MyChurch.Api.Web.Controllers
{
    public class MemberController : BaseController
    {
        /// <summary>
        /// Create a new Member
        /// </summary>
        /// <response code="200">Success: Member Created</response>
        /// <response code="400">Failure: Invalid Requet</response>
        /// <response code="401">Failure: error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateChurch(CreateChurchCommand command)
        {
            var mediator = await Mediator.Send(command);
            return Ok(mediator);
        }
    }
}
