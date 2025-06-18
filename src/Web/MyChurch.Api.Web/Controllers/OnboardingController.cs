using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Onboarding.IdentifyMember;
using MyChurch.Application.Onboarding.ActivateAccount;
using MyChurch.Application.Onboarding.RegisterForApproval;
using MyChurch.Application.Member.Commands.ActiveMemberPassword;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnboardingController : BaseController
    {
        [HttpPost("identify")]
        [AllowAnonymous]
        public async Task<IActionResult> Identify([FromBody] IdentifyMemberCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("activate/validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateForActivation([FromBody] ValidateMemberForActivationCommand command)
        {
            var hash = await Mediator.Send(command);
            return Ok(new { Hash = hash });
        }

        [HttpPatch("activate/password/{hash}")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivatePassword(string hash, [FromBody] ActiveMemberPasswordCommand command)
        {
            command.Hash = hash;
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterForApprovalCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}