using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateChurchCommand;
using MyChurch.Application.Member.Commands.ActiveMemberPassword;
using MyChurch.Application.Member.Commands.CreateMember;

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
        public async Task<IActionResult> CreateMember(CreateMemberCommand command)
        {
            var mediator = await Mediator.Send(command);
            return Ok(mediator);
        }
        /// <summary>
        /// Active Password
        /// </summary>
        /// <response code="204">Success: Password Activaded</response>
        /// <response code="400">Failure: Invalid Requet</response>
        /// <response code="401">Failure: error</response>
        [HttpPatch("active/password/{hash}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ActivePassword( string hash, ActiveMemberPasswordCommand command)
        {
            command.Hash = hash;
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
