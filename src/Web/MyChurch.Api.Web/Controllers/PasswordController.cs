using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Member.Commands.ChangeMyPassword;
using MyChurch.Application.Member.Commands.ForgotPassword;

namespace MyChurch.Api.Controllers
{
    [ApiController]
    [Route("api/password")]
    public class PasswordController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PasswordController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> Forgot([FromBody] RequestPasswordResetCommand command)
        {
            await _mediator.Send(command);
            // resposta genérica (não revela se o email existe)
            return Ok(new { message = "Se existir uma conta com este email, enviaremos instruções para redefinir a senha." });
        }

        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> Reset([FromBody] ConfirmPasswordResetCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Senha redefinida com sucesso." });
        }

        [HttpPost("change")]
        [Authorize]
        public async Task<IActionResult> Change([FromBody] ChangeMyPasswordCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Senha alterada com sucesso." });
        }
    }
}
