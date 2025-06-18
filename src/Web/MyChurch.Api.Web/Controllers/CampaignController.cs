using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Campaign.Commands;
using MyChurch.Application.Campaign.Queries;
using MyChurch.Application.Dtos;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCampaignCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCampaignCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = AuthorizationRequestCreate<DeleteCampaignCommand>();
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpGet]
        [Authorize] // Agora exige autenticação
        public async Task<IActionResult> GetAll()
        {
            var query = AuthorizationRequestCreate<GetAllActiveCampaignsQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize] // Agora exige autenticação
        public async Task<IActionResult> GetById(int id)
        {
            var query = AuthorizationRequestCreate<GetCampaignDetailsByIdQuery>();
            query.Id = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
