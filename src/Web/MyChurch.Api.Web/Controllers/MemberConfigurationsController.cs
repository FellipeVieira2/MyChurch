using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.Member.Commands.UpdateMemberConfiguration;
using MyChurch.Application.Member.Queries.GetMemberConfiguration;

namespace MyChurch.Api.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MemberConfigurationsController : BaseController
    {
        /// <summary>
        /// Obtém as configurações do membro autenticado.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberConfigurationDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMemberConfiguration()
        {
            var query = AuthorizationRequestCreate<GetMemberConfigurationQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza as configurações do membro autenticado.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberConfigurationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMemberConfiguration([FromBody] UpdateMemberConfigurationCommand command)
        {
            var cmd = AuthorizationRequestCreate<UpdateMemberConfigurationCommand>();
            
            // Copia as propriedades enviadas
            if (command.PreferredBibleVersionId.HasValue)
                cmd.PreferredBibleVersionId = command.PreferredBibleVersionId;
            
            if (!string.IsNullOrEmpty(command.ThemePreference))
                cmd.ThemePreference = command.ThemePreference;
            
            if (!string.IsNullOrEmpty(command.FontSize))
                cmd.FontSize = command.FontSize;
            
            if (command.EnableNotifications.HasValue)
                cmd.EnableNotifications = command.EnableNotifications;
            
            var result = await Mediator.Send(cmd);
            return Ok(result);
        }
    }
}