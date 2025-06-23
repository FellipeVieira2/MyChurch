using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.BibleReadingPlan.Commands;
using MyChurch.Application.BibleReadingPlan.Queries;
using MyChurch.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BibleReadingPlanController : BaseController
    {
        /// <summary>
        /// Lista todos os planos de leitura bíblica disponíveis
        /// </summary>
        /// <param name="includeStages">Se true, inclui as etapas detalhadas de cada plano</param>
        /// <response code="200">Sucesso: Lista de planos de leitura bíblica</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<BibleReadingPlanDto>))]
        public async Task<IActionResult> GetAllPlans([FromQuery] bool includeStages = false)
        {
            var query = AuthorizationRequestCreate<GetAllBibleReadingPlansQuery>();
            query.IncludeStages = includeStages;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtém detalhes de um plano de leitura bíblica específico
        /// </summary>
        /// <param name="id">ID do plano de leitura</param>
        /// <response code="200">Sucesso: Detalhes do plano</response>
        /// <response code="404">Falha: Plano não encontrado</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BibleReadingPlanDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlanDetails(int id)
        {
            var query = AuthorizationRequestCreate<GetBibleReadingPlanDetailsQuery>();
            query.PlanId = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Atribui um plano de leitura bíblica a um membro
        /// </summary>
        /// <param name="planId">ID do plano a ser atribuído</param>
        /// <response code="204">Sucesso: Plano atribuído</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("assign/{planId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignPlan(int planId)
        {
            var command = AuthorizationRequestCreate<AssignBibleReadingPlanCommand>();
            command.PlanId = planId;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Marca uma etapa de leitura bíblica como concluída
        /// </summary>
        /// <param name="planId">ID do plano de leitura</param>
        /// <param name="stageId">ID da etapa concluída</param>
        /// <response code="204">Sucesso: Etapa marcada como concluída</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("{planId}/complete-stage/{stageId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteStage(int planId, int stageId)
        {
            var command = AuthorizationRequestCreate<CompleteBibleReadingStageCommand>();
            command.PlanId = planId;
            command.StageId = stageId;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Obtém o progresso do membro em um plano de leitura bíblica específico
        /// </summary>
        /// <param name="planId">ID do plano de leitura</param>
        /// <response code="200">Sucesso: Progresso do membro no plano</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("progress/{planId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BibleReadingPlanProgressDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMemberProgress(int planId)
        {
            var query = AuthorizationRequestCreate<GetMemberReadingProgressQuery>();
            query.PlanId = planId;
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}