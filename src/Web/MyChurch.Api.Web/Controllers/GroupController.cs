using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyChurch.Application.Group.Commands;
using MyChurch.Application.Group.Queries;
using MyChurch.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyChurch.Api.Web.Middleware;

namespace MyChurch.Api.Web.Controllers
{
    /// <summary>
    /// Gerencia operações de grupos (células, ministérios, equipes, etc).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : BaseController
    {
        private readonly IHubContext<GroupHub> _hubContext;

        public GroupController(IHubContext<GroupHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Cria um novo grupo (célula, ministério, equipe, etc).
        /// </summary>
        /// <remarks>O usuário autenticado será o líder do grupo.</remarks>
        /// <response code="200">Sucesso: ID do grupo criado</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(id);
        }

        /// <summary>
        /// Adiciona um membro ao grupo (apenas líder pode adicionar).
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("add-member/{groupId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMember([FromRoute] int groupId, [FromBody] AddMemberToGroupCommand command)
        {
            command.GroupId = groupId;
            await Mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Atualiza os dados do grupo (apenas líder pode atualizar).
        /// </summary>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPut("{groupId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGroup([FromRoute] int groupId, [FromBody] UpdateGroupCommand command)
        {
            command.GroupId = groupId;
            await Mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Registra presença dos membros em uma reunião do grupo (apenas líder pode registrar).
        /// </summary>
        /// <remarks>Envia a lista de presenças dos membros para o evento/reunião do grupo.</remarks>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("register-attendance/{groupId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterAttendance([FromRoute] int groupId, [FromBody] RegisterGroupAttendanceCommand command)
        {
            command.GroupId = groupId;
            await Mediator.Send(command);
            return Ok();
        }

        /// <summary>
        /// Cria uma nova reunião de grupo e notifica via websocket
        /// </summary>
        /// <response code="200">Sucesso: ID da reunião criada</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("meeting")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateGroupMeeting([FromBody] CreateGroupMeetingCommand command)
        {
            var meetingId = await Mediator.Send(command);
            var newMeetingDto = new { Id = meetingId, command.GroupId, command.MeetingDate, command.Topic, command.SummaryNotes };
            await _hubContext.Clients.Group(command.GroupId.ToString()).SendAsync("NewMeetingScheduled", newMeetingDto);
            return Ok(meetingId);
        }
    }
}
