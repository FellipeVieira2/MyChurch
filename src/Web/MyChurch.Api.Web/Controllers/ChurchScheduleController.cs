using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateChurchSchedule;
using MyChurch.Application.Church.Commands.DeleteChurchSchedule;
using MyChurch.Application.Church.Commands.UpdateChurchSchedule;
using MyChurch.Application.Church.Queries.GetChurchSchedules;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Enum;
using System.Security.Claims;

namespace MyChurch.Api.Web.Controllers
{
    /// <summary>
    /// Gerenciamento de horários de cultos e atividades das igrejas
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ChurchScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChurchScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Cria um novo horário de culto/atividade
        /// </summary>
        /// <param name="command">Dados do horário</param>
        /// <returns>Horário criado</returns>
        /// <response code="200">Horário criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autenticado</response>
        /// <response code="403">Sem permissão (apenas Admin)</response>
        [HttpPost]
        [ProducesResponseType(typeof(ChurchScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateChurchScheduleCommand command)
        {
            InjectUserContext(command);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os horários de uma igreja
        /// </summary>
        /// <param name="churchId">ID da igreja</param>
        /// <param name="onlyActive">Retornar apenas horários ativos (padrão: true)</param>
        /// <returns>Lista de horários</returns>
        /// <response code="200">Retorna lista de horários</response>
        /// <response code="401">Não autenticado</response>
        [HttpGet("church/{churchId}")]
        [AllowAnonymous] // Permite visitantes consultarem horários
        [ProducesResponseType(typeof(List<ChurchScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByChurch(int churchId, [FromQuery] bool onlyActive = true)
        {
            var query = new GetChurchSchedulesQuery 
            { 
                ChurchId = churchId,
                OnlyActive = onlyActive
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza um horário existente
        /// </summary>
        /// <param name="id">ID do horário</param>
        /// <param name="command">Dados para atualização</param>
        /// <returns>Horário atualizado</returns>
        /// <response code="200">Horário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autenticado</response>
        /// <response code="403">Sem permissão (apenas Admin)</response>
        /// <response code="404">Horário não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ChurchScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChurchScheduleCommand command)
        {
            command.Id = id;
            InjectUserContext(command);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Remove um horário
        /// </summary>
        /// <param name="id">ID do horário</param>
        /// <param name="churchId">ID da igreja</param>
        /// <returns>Sem conteúdo</returns>
        /// <response code="204">Horário removido com sucesso</response>
        /// <response code="401">Não autenticado</response>
        /// <response code="403">Sem permissão (apenas Admin)</response>
        /// <response code="404">Horário não encontrado</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] int churchId)
        {
            var command = new DeleteChurchScheduleCommand 
            { 
                Id = id,
                ChurchId = churchId
            };
            InjectUserContext(command);
            await _mediator.Send(command);
            return NoContent();
        }

        private void InjectUserContext(JwtMemberDto command)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                command.UserId = userId;
            }
            command.Email = User.FindFirst(ClaimTypes.Email)?.Value;
            command.Role = User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
