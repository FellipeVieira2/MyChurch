using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Hymn.Commands.ImportHymnJson;
using MyChurch.Application.Hymn.Queries.GetHymnByNumber;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HymnController : BaseController
    {
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
        /// Busca um hino pelo número, retornando o coro e os versos.
        /// </summary>
        [HttpGet("{number}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHymnByNumber(int number)
        {
            var query = new GetHymnByNumberQuery { Number = number };
            var result = await Mediator.Send(query);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
