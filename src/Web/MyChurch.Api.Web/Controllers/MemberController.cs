using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Application.Member.Commands.ActiveMemberPassword;
using MyChurch.Application.Member.Commands.CreateMember;
using MyChurch.Application.Member.Queries.GetAllMembers;
using MyChurch.Application.Member.Queries.GetMemberById;

namespace MyChurch.Api.Web.Controllers
{
    public class MemberController : BaseController
    {
        /// <summary>
        /// Create a new Member
        /// </summary>
        /// <response code="200">Success: Member Created</response>
        /// <response code="400">Failure: Invalid Requet</response>
        /// <response code="401">Failure: error</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateMember(CreateMemberCommand command)
        {
            var mediator = await Mediator.Send(command);
            return Ok(mediator);
        }
        /// <summary>
        /// Active Password
        /// </summary>
        /// <response code="204">Success: Password Activaded</response>
        /// <response code="400">Failure: Invalid Requet</response>
        /// <response code="401">Failure: error</response>
        [HttpPatch("active/password/{hash}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ActivePassword( string hash, ActiveMemberPasswordCommand command)
        {
            command.Hash = hash;
            await Mediator.Send(command);
            return NoContent();
        }
        /// <summary>
        /// Get Member by ID
        /// </summary>
        /// <response code="200">Success: Member Retrieved</response>
        /// <response code="400">Failure: Invalid Request</response>
        /// <response code="401">Failure: Unauthorized</response>
        /// <response code="404">Failure: Member Not Found</response>
        [HttpGet("{id}")]
        [Authorize()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMember([FromRoute] int id)
        {
            var query = AuthorizationRequestCreate<GetMemberByIdQuery>();
            query.Id = id;
            var member = await Mediator.Send(query);

            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }

        /// <summary>
        /// Lista membros da igreja com filtros e paginação (apenas para Admin)
        /// </summary>
        /// <response code="200">Sucesso: Lista paginada de membros</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResultDto<MemberDto>))]
        public async Task<IActionResult> GetAllMembers([FromQuery] GetAllMembersQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
