using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Bible.Commands.ImportBibleVersion;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BibleVersionController : BaseController
    {
        /// <summary>
        /// Importa uma versão da Bíblia a partir de um arquivo JSON.
        /// </summary>
        /// <param name="command">Command para importação da versão da Bíblia</param>
        /// <returns></returns>
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportBibleVersion([FromForm] ImportBibleVersionCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}