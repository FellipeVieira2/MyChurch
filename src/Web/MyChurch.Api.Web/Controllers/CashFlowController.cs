using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.CashFlow.Commands.CreateCashFlowEntry;
using MyChurch.Application.CashFlow.Commands.UpdateCashFlowEntry;
using MyChurch.Application.CashFlow.Commands.DeleteCashFlowEntry;
using MyChurch.Application.CashFlow.Queries.GetCashFlowEntryById;
using MyChurch.Application.CashFlow.Queries.GetAllCashFlowEntries;
using MyChurch.Application.Dtos;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.CashFlow.Queries.GetChurchCashFlowBalance;
using MyChurch.Application.CashFlow.Commands.CreateCashFlowCategory;
using MyChurch.Application.CashFlow.Commands.UpdateCashFlowCategory;
using MyChurch.Application.CashFlow.Commands.DeleteCashFlowCategory;
using MyChurch.Application.CashFlow.Queries.GetAllCashFlowCategories;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashFlowController : BaseController
    {
        /// <summary>
        /// Cria um novo lançamento de fluxo de caixa
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateCashFlowEntry([FromBody] CreateCashFlowEntryCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retorna o saldo consolidado do fluxo de caixa da igreja
        /// </summary>
        [Authorize]
        [HttpGet("balance")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        public async Task<IActionResult> GetChurchCashFlowBalance()
        {
            // Quando implementar, crie uma query como GetChurchCashFlowBalanceQuery
            var saldo = await Mediator.Send(AuthorizationRequestCreate<GetChurchCashFlowBalanceQuery>());
            return Ok(saldo);
        }

        /// <summary>
        /// Atualiza um lançamento de fluxo de caixa existente
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CashFlowEntryDto))]
        public async Task<IActionResult> UpdateCashFlowEntry(int id, [FromBody] UpdateCashFlowEntryCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Busca um lançamento de fluxo de caixa por ID
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CashFlowEntryDto))]
        public async Task<IActionResult> GetCashFlowEntry(int id)
        {
            var query = AuthorizationRequestCreate<GetCashFlowEntryByIdQuery>();
            query.Id = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista lançamentos de fluxo de caixa
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDto<CashFlowEntryDto>))]
        public async Task<IActionResult> GetAllCashFlowEntries([FromQuery] GetAllCashFlowEntriesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Exclui um lançamento de fluxo de caixa
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCashFlowEntry(int id)
        {
            var command = AuthorizationRequestCreate<DeleteCashFlowEntryCommand>();
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Cria uma nova categoria de fluxo de caixa
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateCashFlowCategory([FromBody] CreateCashFlowCategoryCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza uma categoria de fluxo de caixa existente
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("categories/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CashFlowCategoryDto))]
        public async Task<IActionResult> UpdateCashFlowCategory(int id, [FromBody] UpdateCashFlowCategoryCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Exclui uma categoria de fluxo de caixa
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("categories/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCashFlowCategory(int id)
        {
            var command = AuthorizationRequestCreate<DeleteCashFlowCategoryCommand>();
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Lista todas as categorias de fluxo de caixa da igreja
        /// </summary>
        [Authorize]
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CashFlowCategoryDto>))]
        public async Task<IActionResult> GetAllCashFlowCategories()
        {
            var query = AuthorizationRequestCreate<GetAllCashFlowCategoriesQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
