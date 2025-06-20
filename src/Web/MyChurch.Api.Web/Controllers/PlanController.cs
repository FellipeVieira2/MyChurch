using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plan.Commands.CreatePlan;
using MyChurch.Application.Plan.Commands.DeletePlan;
using MyChurch.Application.Plan.Commands.UpdatePlan;
using MyChurch.Application.Plan.Queries.GetAllPlans;
using MyChurch.Application.Plan.Queries.GetPlanById;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : BaseController
    {
        /// <summary>
        /// Lista todos os planos disponíveis
        /// </summary>
        /// <response code="200">Sucesso: Lista de planos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PlanDto>))]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllPlansQuery();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Busca um plano por ID
        /// </summary>
        /// <response code="200">Sucesso: Plano encontrado</response>
        /// <response code="404">Falha: Plano não encontrado</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlanDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetPlanByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Cria um novo plano (somente para administradores da plataforma)
        /// </summary>
        /// <response code="200">Sucesso: ID do plano criado</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost]
        [Authorize(Roles = "PlatformAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> Create([FromBody] CreatePlanCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza um plano existente (somente para administradores da plataforma)
        /// </summary>
        /// <response code="200">Sucesso: Plano atualizado</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="404">Falha: Plano não encontrado</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "PlatformAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlanDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlanCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Exclui um plano (somente para administradores da plataforma)
        /// </summary>
        /// <response code="200">Sucesso: Plano excluído</response>
        /// <response code="400">Falha: Plano em uso</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="404">Falha: Plano não encontrado</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "PlatformAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = AuthorizationRequestCreate<DeletePlanCommand>();
            command.Id = id;
            await Mediator.Send(command);
            return Ok();
        }
    }
}