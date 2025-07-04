using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.ImportedHymn.Commands;
using MyChurch.Application.ImportedHymn.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    public class ImportedHymnController : BaseController
    {
        public ImportedHymnController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<ActionResult<List<ImportedHymnDto>>> GetAll()
        {
            var result = await Mediator.Send(new GetAllImportedHymnsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImportedHymnDto>> GetById(int id)
        {
            var result = await Mediator.Send(new GetImportedHymnByIdQuery { Id = id });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateImportedHymnCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateImportedHymnCommand command)
        {
            command.Id = id;
            var success = await Mediator.Send(command);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await Mediator.Send(new DeleteImportedHymnCommand { Id = id });
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
