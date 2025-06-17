using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Subscription.Commands.CreateSubscription;
// Importe os comandos/queries corretos para Subscription
// using MyChurch.Application.Subscription.Commands.UpdateSubscription;
// using MyChurch.Application.Subscription.Queries.GetSubscription;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : BaseController
    {
        /// <summary>
        /// Cria uma nova assinatura
        /// </summary>
        /// <response code="200">Sucesso: Assinatura criada</response>
        /// <response code="400">Falha: Requisição inválida</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza uma assinatura existente
        /// </summary>
        /// <response code="200">Sucesso: Assinatura atualizada</response>
        /// <response code="400">Falha: Requisição inválida</response>
        [Authorize(Roles = "PlatformAdmin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK/*, Type = typeof(SubscriptionDto)*/)]
        public async Task<IActionResult> UpdateSubscription(int id/*, [FromBody] UpdateSubscriptionCommand command*/)
        {
            // command.Id = id;
            // var result = await Mediator.Send(command);
            // return Ok(result);
            return Ok(); // Remova após implementar
        }

        /// <summary>
        /// Busca uma assinatura por ID
        /// </summary>
        /// <response code="200">Sucesso: Assinatura retornada</response>
        /// <response code="404">Falha: Não encontrada</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK/*, Type = typeof(SubscriptionDto)*/)]
        public async Task<IActionResult> GetSubscription(int id)
        {
            // var query = new GetSubscriptionByIdQuery { Id = id };
            // var result = await Mediator.Send(query);
            // return Ok(result);
            return Ok(); // Remova após implementar
        }

        /// <summary>
        /// Cancela uma assinatura
        /// </summary>
        /// <response code="200">Sucesso: Assinatura cancelada</response>
        /// <response code="404">Falha: Não encontrada</response>
        [Authorize(Roles = "PlatformAdmin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelSubscription(int id)
        {
            // var command = new CancelSubscriptionCommand { Id = id };
            // await Mediator.Send(command);
            // return Ok();
            return Ok(); // Remova após implementar
        }

        /// <summary>
        /// Troca o plano da igreja (upgrade/downgrade)
        /// </summary>
        /// <response code="200">Sucesso: Plano alterado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [Authorize(Roles = "Admin")]
        [HttpPost("change-plan")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChangeChurchPlanResultDto))]
        public async Task<IActionResult> ChangePlan([FromBody] ChangeChurchPlanCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}