using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Common.Models;
using MyChurch.Application.Dtos;
using MyChurch.Application.CashFlow.Commands.CreateCashFlowEntry;
using MyChurch.Application.CashFlow.Commands.UpdateCashFlowEntry;
using MyChurch.Application.CashFlow.Commands.DeleteCashFlowEntry;
using MyChurch.Application.CashFlow.Queries.GetCashFlowEntryById;
using MyChurch.Application.CashFlow.Queries.GetAllCashFlowEntries;
using MyChurch.Application.CashFlow.Queries.GetChurchCashFlowBalance;
using MyChurch.Application.CashFlow.Commands.CreateCashFlowCategory;
using MyChurch.Application.CashFlow.Commands.UpdateCashFlowCategory;
using MyChurch.Application.CashFlow.Commands.DeleteCashFlowCategory;
using MyChurch.Application.CashFlow.Queries.GetAllCashFlowCategories;
using MyChurch.Domain.Enum;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashFlowController : BaseController
    {
        /// <summary>
        /// Cria um novo lançamento de fluxo de caixa
        /// </summary>
        [Authorize(Roles = UserRoleAccess.FinancialManageRoles)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateCashFlowEntry([FromBody] CreateCashFlowEntryCommand command)
        {
            var c = AuthorizationRequestCreate<CreateCashFlowEntryCommand>();
            c.Amount = command.Amount;
            c.Date = command.Date;
            c.Description = command.Description;
            c.Type = command.Type;
            c.CategoryId = command.CategoryId;
            c.DepartmentId = command.DepartmentId;

            var result = await Mediator.Send(c);
            return Ok(result);
        }

        /// <summary>
        /// Retorna o saldo consolidado do fluxo de caixa da igreja com totalizadores
        /// </summary>
        [Authorize(Roles = UserRoleAccess.FinancialViewRoles)]
        [HttpGet("balance")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
        public async Task<IActionResult> GetChurchCashFlowBalance()
        {
            var saldo = await Mediator.Send(AuthorizationRequestCreate<GetChurchCashFlowBalanceQuery>());
            return Ok(saldo);
        }

        /// <summary>
        /// Atualiza um lançamento de fluxo de caixa existente
        /// </summary>
        [Authorize(Roles = UserRoleAccess.FinancialManageRoles)]
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
        [Authorize(Roles = UserRoleAccess.FinancialViewRoles)]
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
        /// Lista lançamentos de fluxo de caixa com paginação, filtros e ordenação.
        /// Retorna também os totais de entrada, saída e saldo.
        /// </summary>
        /// <param name="query">Filtros de busca e paginação</param>
        /// <returns>Lista paginada com totalizadores</returns>
        [Authorize(Roles = UserRoleAccess.FinancialViewRoles)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CashFlowEntryPagedResult))]
        public async Task<IActionResult> GetAllCashFlowEntries([FromQuery] GetAllCashFlowEntriesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Exclui um lançamento de fluxo de caixa
        /// </summary>
        [Authorize(Roles = UserRoleAccess.FinancialManageRoles)]
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
        [Authorize(Roles = UserRoleAccess.FinancialManageRoles)]
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
        [Authorize(Roles = UserRoleAccess.FinancialManageRoles)]
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
        [Authorize(Roles = UserRoleAccess.FinancialManageRoles)]
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
        /// Lista todas as categorias de fluxo de caixa da igreja com paginação e filtros
        /// </summary>
        /// <param name="query">Filtros e paginação</param>
        /// <returns>Lista paginada de categorias</returns>
        [Authorize(Roles = UserRoleAccess.FinancialViewRoles)]
        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<CashFlowCategoryDto>))]
        public async Task<IActionResult> GetAllCashFlowCategories([FromQuery] GetAllCashFlowCategoriesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
