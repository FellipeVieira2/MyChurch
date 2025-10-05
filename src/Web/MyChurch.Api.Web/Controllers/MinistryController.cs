using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.Ministries.Commands.AddMinistryMember;
using MyChurch.Application.Ministries.Commands.CreateMinistry;
using MyChurch.Application.Ministries.Commands.DeleteMinistry;
using MyChurch.Application.Ministries.Commands.RemoveMinistryMember;
using MyChurch.Application.Ministries.Commands.UpdateMinistry;
using MyChurch.Application.Ministries.Queries.GetMinistriesByChurch;
using MyChurch.Application.Ministries.Queries.GetMinistryById;
using MyChurch.Application.Ministries.Queries.GetMinistryMembers;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MinistryController : BaseController
    {
        /// <summary>
        /// Cria um novo ministério
        /// </summary>
        /// <response code="200">Sucesso: Ministério criado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinistryDto))]
        public async Task<IActionResult> CreateMinistry([FromBody] CreateMinistryCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza um ministério existente
        /// </summary>
        /// <response code="200">Sucesso: Ministério atualizado</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="404">Falha: Ministério não encontrado</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinistryDto))]
        public async Task<IActionResult> UpdateMinistry(int id, [FromBody] UpdateMinistryCommand command)
        {
            command.Id = id;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Deleta um ministério
        /// </summary>
        /// <response code="204">Sucesso: Ministério deletado</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="404">Falha: Ministério não encontrado</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMinistry(int id)
        {
            var command = new DeleteMinistryCommand { Id = id };
            var result = await Mediator.Send(command);
            
            if (!result)
                return NotFound();
                
            return NoContent();
        }

        /// <summary>
        /// Busca um ministério por ID
        /// </summary>
        /// <response code="200">Sucesso: Ministério retornado</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="404">Falha: Ministério não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinistryDto))]
        public async Task<IActionResult> GetMinistryById(int id)
        {
            var query = new GetMinistryByIdQuery { Id = id };
            var result = await Mediator.Send(query);
            
            if (result == null)
                return NotFound();
                
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os ministérios de uma igreja
        /// </summary>
        /// <response code="200">Sucesso: Lista de ministérios</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("church/{churchId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MinistryDto>))]
        public async Task<IActionResult> GetMinistriesByChurch(int churchId, [FromQuery] bool? isActive = null)
        {
            var query = new GetMinistriesByChurchQuery 
            { 
                ChurchId = churchId,
                IsActive = isActive
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os membros de um ministério
        /// </summary>
        /// <response code="200">Sucesso: Lista de membros</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("{ministryId}/members")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MinistryMemberDto>))]
        public async Task<IActionResult> GetMinistryMembers(int ministryId, [FromQuery] bool? isActive = null)
        {
            var query = new GetMinistryMembersQuery 
            { 
                MinistryId = ministryId,
                IsActive = isActive
            };
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Adiciona um membro a um ministério
        /// </summary>
        /// <response code="200">Sucesso: Membro adicionado ao ministério</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("{ministryId}/members")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinistryMemberDto))]
        public async Task<IActionResult> AddMinistryMember(int ministryId, [FromBody] AddMinistryMemberCommand command)
        {
            command.MinistryId = ministryId;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Remove um membro de um ministério
        /// </summary>
        /// <response code="204">Sucesso: Membro removido do ministério</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="404">Falha: Membro não encontrado no ministério</response>
        [HttpDelete("{ministryId}/members/{memberId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveMinistryMember(int ministryId, int memberId)
        {
            var command = new RemoveMinistryMemberCommand 
            { 
                MinistryId = ministryId, 
                MemberId = memberId 
            };
            var result = await Mediator.Send(command);
            
            if (!result)
                return NotFound();
                
            return NoContent();
        }
    }
}
