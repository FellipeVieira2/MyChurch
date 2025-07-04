using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Application.Member.Commands.ActiveMemberPassword;
using MyChurch.Application.Member.Commands.AdminChangePassword;
using MyChurch.Application.Member.Commands.CreateMember;
using MyChurch.Application.Member.Commands.UpdateMember;
using MyChurch.Application.Member.Commands.CreateMemberImportFile;
using MyChurch.Application.Member.Queries.GetAllCreditCardsMember;
using MyChurch.Application.Member.Queries.GetAllMembers;
using MyChurch.Application.Member.Queries.GetBirthdayMembers;
using MyChurch.Application.Member.Queries.GetMemberById;
using MyChurch.Application.Member.Queries.GetMemberCounts;

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
        /// Altera a senha de um membro (apenas para administradores)
        /// </summary>
        /// <response code="204">Sucesso: Senha alterada</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        /// <response code="404">Falha: Membro não encontrado</response>
        [HttpPut("{id}/change-password")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AdminChangePassword([FromRoute] int id, [FromBody] AdminChangePasswordCommand command)
        {
            var cmd = AuthorizationRequestCreate<AdminChangePasswordCommand>();
            cmd.MemberId = id;
            cmd.NewPassword = command.NewPassword;
            
            await Mediator.Send(cmd);
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
        /// Lista os membros aniversariantes filtrando por dia, semana ou mês
        /// </summary>
        /// <param name="filterType">Day, Week ou Month</param>
        /// <response code="200">Sucesso: Lista de aniversariantes</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("birthdays")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<MemberDto>))]
        public async Task<IActionResult> GetBirthdayMembers([FromQuery] BirthdayFilterType filterType)
        {
            var query = AuthorizationRequestCreate<GetBirthdayMembersQuery>();
            query.FilterType = filterType;
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Update Member by ID
        /// </summary>
        /// <response code="200">Success: Member Updated</response>
        /// <response code="400">Failure: Invalid Request</response>
        /// <response code="401">Failure: Unauthorized</response>
        /// <response code="404">Failure: Member Not Found</response>
        [HttpPut("{id}")]
        [Authorize()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMember([FromRoute] int id, [FromBody] UpdateMemberCommand command)
        {

            command.Id = id;
            var member = await Mediator.Send(command);

            return Ok(member);
        }

        /// <summary>
        /// Get all Credit Cards
        /// </summary>
        /// <response code="200">Success: Credit Cards</response>
        /// <response code="400">Failure: Invalid Request</response>
        /// <response code="401">Failure: Unauthorized</response>
        /// <response code="404">Failure: Member Not Found</response>
        [HttpGet("credit-cards")]
        [Authorize()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCreditCards([FromQuery] GetAllCreditCardsMemberQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
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

        /// <summary>
        /// Importa membros por arquivo CSV
        /// </summary>
        /// <param name="file">Arquivo CSV</param>
        /// <response code="200">Sucesso: IDs dos membros criados</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("import-csv")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<int>))]
        public async Task<IActionResult> ImportMembersCsv([FromForm] RecieveCsvFile file)
        {
            var command = AuthorizationRequestCreate<CreateMemberImportFileCommand>();
            command.CsvFile = file.CsvFile;
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Retorna o total de membros, total ativos e total inativos
        /// </summary>
        /// <response code="200">Sucesso: Totais de membros</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("counts")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MemberCountDto))]
        public async Task<IActionResult> GetMemberCounts()
        {
            var query = AuthorizationRequestCreate<GetMemberCountsQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Aprova o cadastro de um membro (apenas para administradores)
        /// </summary>
        /// <response code="204">Sucesso: Membro aprovado</response>
        /// <response code="404">Falha: Membro não encontrado</response>
        [HttpPut("{memberId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRegistration(string memberId)
        {
            var command = new MyChurch.Application.Member.Commands.ApproveMemberRegistration.ApproveMemberRegistrationCommand { MemberId = memberId };
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Reprova o cadastro de um membro (apenas para administradores)
        /// </summary>
        /// <response code="204">Sucesso: Membro reprovado</response>
        /// <response code="404">Falha: Membro não encontrado</response>
        [HttpPut("{memberId}/decline")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeclineRegistration(string memberId, [FromBody] string? reason = null)
        {
            var command = new MyChurch.Application.Member.Commands.ApproveMemberRegistration.DeclineMemberRegistrationCommand { MemberId = memberId, Reason = reason };
            await Mediator.Send(command);
            return NoContent();
        }

        public class RecieveCsvFile()
        {
            public IFormFile CsvFile
            {
                get; set;
            }
        }
    }
}
