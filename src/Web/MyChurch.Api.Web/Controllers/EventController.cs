using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Application.Event.Commands.CreateEvent;
using MyChurch.Application.Event.Commands.DeleteEvent;
using MyChurch.Application.Event.Commands.ManageDiaconateScale;
using MyChurch.Application.Event.Commands.ManageKidsScale;
using MyChurch.Application.Event.Commands.UpdateEvent;
using MyChurch.Application.Event.Commands.ManageEventParticipants;
using MyChurch.Application.Event.Queires.GetAllWorship;
using MyChurch.Application.Event.Queires.GetEventsForCalendar;
using MyChurch.Application.Event.Queires.GetWorshipById;
using MyChurch.Application.Event.Queries.GetDiaconateScaleMembers;
using MyChurch.Application.Event.Queries.GetEventById;
using MyChurch.Application.Event.Queries.GetKidsScaleMembers;
using MyChurch.Application.WorshipService.Commands.ManageSchedule;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : BaseController
    {
        /// <summary>
        /// Cria um novo evento
        /// </summary>
        /// <response code="200">Sucesso: Evento criado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza um evento existente
        /// </summary>
        /// <response code="200">Sucesso: Evento atualizado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventDto))]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] UpdateEventCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza a lista de participantes do evento (substitui a lista inteira).
        /// </summary>
        /// <response code="200">Sucesso: Evento atualizado com lista de participantes</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="403">Falha: Apenas Admin</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/participants")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventDto))]
        public async Task<IActionResult> UpdateParticipants([FromRoute] int id, [FromBody] List<int> participantIds)
        {
            var cmd = AuthorizationRequestCreate<ManageEventParticipantsCommand>();
            cmd.EventId = id;
            cmd.ParticipantIds = participantIds ?? new List<int>();

            var updated = await Mediator.Send(cmd);
            return Ok(updated);
        }

        [Authorize]
        [HttpGet("{id}/diaconate-scale")]
        public async Task<IActionResult> GetDiaconateScale([FromRoute] int id)
        {
            var query = AuthorizationRequestCreate<GetDiaconateScaleMembersQuery>();
            query.EventId = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/diaconate-scale")]
        public async Task<IActionResult> AddDiaconateScale([FromRoute] int id, [FromBody] AddDiaconateScaleMemberCommand command)
        {
            var authorizedCommand = AuthorizationRequestCreate<AddDiaconateScaleMemberCommand>();
            authorizedCommand.EventId = id;
            authorizedCommand.MemberId = command.MemberId;
            authorizedCommand.RoleName = command.RoleName;
            authorizedCommand.Order = command.Order;
            authorizedCommand.Notes = command.Notes;

            var result = await Mediator.Send(authorizedCommand);
            return Ok(new { id = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/diaconate-scale/{scaleItemId}")]
        public async Task<IActionResult> UpdateDiaconateScale([FromRoute] int id, [FromRoute] int scaleItemId, [FromBody] UpdateDiaconateScaleMemberCommand command)
        {
            var authorizedCommand = AuthorizationRequestCreate<UpdateDiaconateScaleMemberCommand>();
            authorizedCommand.Id = scaleItemId;
            authorizedCommand.EventId = id;
            authorizedCommand.MemberId = command.MemberId;
            authorizedCommand.RoleName = command.RoleName;
            authorizedCommand.Order = command.Order;
            authorizedCommand.Notes = command.Notes;

            var result = await Mediator.Send(authorizedCommand);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/diaconate-scale/{scaleItemId}")]
        public async Task<IActionResult> RemoveDiaconateScale([FromRoute] int id, [FromRoute] int scaleItemId)
        {
            var command = AuthorizationRequestCreate<RemoveDiaconateScaleMemberCommand>();
            command.Id = scaleItemId;
            command.EventId = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/kids-scale")]
        public async Task<IActionResult> GetKidsScale([FromRoute] int id)
        {
            var query = AuthorizationRequestCreate<GetKidsScaleMembersQuery>();
            query.EventId = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/kids-scale")]
        public async Task<IActionResult> AddKidsScale([FromRoute] int id, [FromBody] AddKidsScaleMemberCommand command)
        {
            var authorizedCommand = AuthorizationRequestCreate<AddKidsScaleMemberCommand>();
            authorizedCommand.EventId = id;
            authorizedCommand.MemberId = command.MemberId;
            authorizedCommand.RoleName = command.RoleName;
            authorizedCommand.Order = command.Order;
            authorizedCommand.Notes = command.Notes;

            var result = await Mediator.Send(authorizedCommand);
            return Ok(new { id = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/kids-scale/{scaleItemId}")]
        public async Task<IActionResult> UpdateKidsScale([FromRoute] int id, [FromRoute] int scaleItemId, [FromBody] UpdateKidsScaleMemberCommand command)
        {
            var authorizedCommand = AuthorizationRequestCreate<UpdateKidsScaleMemberCommand>();
            authorizedCommand.Id = scaleItemId;
            authorizedCommand.EventId = id;
            authorizedCommand.MemberId = command.MemberId;
            authorizedCommand.RoleName = command.RoleName;
            authorizedCommand.Order = command.Order;
            authorizedCommand.Notes = command.Notes;

            var result = await Mediator.Send(authorizedCommand);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}/kids-scale/{scaleItemId}")]
        public async Task<IActionResult> RemoveKidsScale([FromRoute] int id, [FromRoute] int scaleItemId)
        {
            var command = AuthorizationRequestCreate<RemoveKidsScaleMemberCommand>();
            command.Id = scaleItemId;
            command.EventId = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Deleta um evento existente
        /// </summary>
        /// <response code="204">Sucesso: Evento Deletado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var command = AuthorizationRequestCreate<DeleteEventCommand>();
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Busca um evento por ID
        /// </summary>
        /// <response code="200">Sucesso: Evento retornado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EventDto))]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {
            var query = AuthorizationRequestCreate<GetEventByIdQuery>();
            query.Id = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista eventos para o calendário do mês/ano informado (inclui recorrentes)
        /// </summary>
        /// <response code="200">Sucesso: Lista de eventos</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize]
        [HttpGet("calendar")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EventCalendarDto>))]
        public async Task<IActionResult> GetEventsForCalendar([FromQuery] GetEventsForCalendarQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista eventos para o calendário do mês/ano informado (inclui recorrentes)
        /// </summary>
        /// <response code="200">Sucesso: Lista de eventos</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize]
        [HttpGet("worship")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PagedResultDto<WorshipServiceDto>>))]
        public async Task<IActionResult> GetAllWorship([FromQuery] GetAllWorshipQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
        // ...existing actions...

        /// <summary>
        /// Busca um culto (WorshipService) por ID
        /// </summary>
        /// <response code="200">Sucesso: Culto retornado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize]
        [HttpGet("worship/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WorshipServiceDto))]
        public async Task<IActionResult> GetWorshipById([FromRoute] int id)
        {
            var query = AuthorizationRequestCreate<GetWorshipByIdQuery>();
            query.Id = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
