using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.Hymn.Commands.CreateHymn;
using MyChurch.Application.Hymn.Commands.DeleteHymn;
using MyChurch.Application.Hymn.Commands.ImportHymnJson;
using MyChurch.Application.Hymn.Commands.UpdateHymn;
using MyChurch.Application.Hymn.Queries.GetAllHymnSummaries;
using MyChurch.Application.Hymn.Queries.GetHymnById;
using MyChurch.Application.Hymn.Queries.GetHymnByNumber;
using MyChurch.Application.Hymn.Queries.PreviewHymnLayout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HymnController : BaseController
    {
        /// <summary>
        /// Lista os hinos cadastrados na plataforma.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HymnSummaryDto>))]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        {
            var query = new GetAllHymnSummariesQuery { SearchTerm = search };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Importa hinos a partir de um arquivo JSON no formato da Harpa.
        /// </summary>
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportHymns([FromForm] ImportHymnJsonCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Cria um novo hino.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(HymnDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateHymnCommand command)
        {
            var result = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Retorna um hino pelo identificador.
        /// </summary>
        [HttpGet("id/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HymnDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetHymnByIdQuery { Id = id });
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Busca um hino pelo número, retornando o coro e os versos.
        /// </summary>
        [HttpGet("{number:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HymnDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHymnByNumber(int number)
        {
            var query = new GetHymnByNumberQuery { Number = number };
            var result = await Mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Atualiza um hino existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateHymnCommand command)
        {
            command.Id = id;
            var success = await Mediator.Send(command);
            if (!success)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Remove um hino da plataforma.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await Mediator.Send(new DeleteHymnCommand { Id = id });
            if (!success)
                return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Retorna uma lista de todos os hinos com número e título.
        /// </summary>
        [HttpGet("summaries")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<HymnSummaryDto>))]
        public async Task<IActionResult> GetAllHymnSummaries()
        {
            var query = new GetAllHymnSummariesQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Gera uma pré-visualização das telas da apresentação com base no texto informado.
        /// </summary>
        [HttpPost("preview")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(HymnPresentationPreviewDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Preview([FromBody] PreviewHymnLayoutQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
