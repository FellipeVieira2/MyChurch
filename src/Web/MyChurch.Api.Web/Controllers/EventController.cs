using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Event.Commands.CreateEvent;
using MyChurch.Application.Event.Commands.UpdateEvent;
using MyChurch.Application.Event.Queries.GetEventById;
using MyChurch.Application.Dtos;
using MyChurch.Application.Event.Queires.GetEventsForCalendar;
using MyChurch.Application.Event.Commands.DeleteEvent;

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
    }
}
