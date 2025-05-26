using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Asset.Commands.CreateAsset;
using MyChurch.Application.Asset.Commands.DeleteAsset;
using MyChurch.Application.Asset.Commands.UpdateAsset;
using MyChurch.Application.Asset.Queries.GetAllAssets;
using MyChurch.Application.Asset.Queries.GetAssetById;
using MyChurch.Application.Church.Queries.GetChurch;
using MyChurch.Application.Dtos;
// Importe os comandos/queries corretos para Asset
// using MyChurch.Application.Asset.Commands.CreateAsset;
// using MyChurch.Application.Asset.Commands.UpdateAsset;
// using MyChurch.Application.Asset.Commands.DeleteAsset;
// using MyChurch.Application.Asset.Queries.GetAssetById;

namespace MyChurch.Api.Web.Controllers
{
    public class AssetController : BaseController
    {
        /// <summary>
        /// Cria um novo ativo patrimonial
        /// </summary>
        /// <response code="200">Sucesso: Ativo criado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza um ativo patrimonial existente
        /// </summary>
        /// <response code="200">Sucesso: Ativo atualizado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetDto))]
        public async Task<IActionResult> UpdateAsset(int id, [FromBody] UpdateAssetCommand command)
        {
            command.AssetId = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Busca um ativo patrimonial por ID
        /// </summary>
        /// <response code="200">Sucesso: Ativo retornado</response>
        /// <response code="404">Falha: Não encontrado</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetDto))]
        public async Task<IActionResult> GetAsset(int id)
        {
            var query = AuthorizationRequestCreate<GetAssetByIdQuery>();
            query.Id = id;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Busca Lista de Ativos patrimonial
        /// </summary>
        /// <response code="200">Sucesso: Ativo retornado</response>
        /// <response code="404">Falha: Não encontrado</response>
        [Authorize]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AssetDto))]
        public async Task<IActionResult> GetAllAsset([FromQuery] GetAllAssetsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Exclui um ativo patrimonial
        /// </summary>
        /// <response code="204">Sucesso: Ativo excluído</response>
        /// <response code="404">Falha: Não encontrado</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var command = AuthorizationRequestCreate<DeleteAssetCommand>();
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
