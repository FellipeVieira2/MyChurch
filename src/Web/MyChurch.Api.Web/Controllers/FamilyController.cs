using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Family.Commands;
using MyChurch.Application.Family.Queries;
using MyChurch.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    /// <summary>
    /// Gerencia operações de família, convites e filhos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FamilyController : BaseController
    {
        /// <summary>
        /// Envia convite de conexão de cônjuge para outro membro.
        /// </summary>
        /// <remarks>O usuário autenticado será o convidador.</remarks>
        /// <response code="200">Sucesso: ID do convite criado</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("spouse-invitation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> SendSpouseInvitation([FromBody] SendSpouseConnectionRequestCommand command)
        {
            var req = AuthorizationRequestCreate<SendSpouseConnectionRequestCommand>();
            req.InvitedMemberId = command.InvitedMemberId;
            var id = await Mediator.Send(req);
            return Ok(id);
        }

        /// <summary>
        /// Aceita um convite de conexão de cônjuge.
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("spouse-invitation/accept")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AcceptSpouseInvitation([FromBody] AcceptSpouseConnectionRequestCommand command)
        {
            var req = AuthorizationRequestCreate<AcceptSpouseConnectionRequestCommand>();
            req.InvitationId = command.InvitationId;
            await Mediator.Send(req);
            return Ok();
        }

        /// <summary>
        /// Recusa um convite de conexão de cônjuge.
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("spouse-invitation/decline")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeclineSpouseInvitation([FromBody] DeclineSpouseConnectionRequestCommand command)
        {
            var req = AuthorizationRequestCreate<DeclineSpouseConnectionRequestCommand>();
            req.InvitationId = command.InvitationId;
            await Mediator.Send(req);
            return Ok();
        }

        /// <summary>
        /// Adiciona um filho à família do usuário autenticado.
        /// </summary>
        /// <response code="200">Sucesso: ID do filho criado</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("child")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> AddChild([FromBody] AddChildToFamilyCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(id);
        }
    }
}
