using QRCoder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateChurchCommand;
using MyChurch.Application.Church.Commands.CreateChurchWithAdminMember;
using MyChurch.Application.Church.Commands.UpdateBankingInfo;
using MyChurch.Application.Church.Commands.UpdateChurch;
using MyChurch.Application.Church.Queries.GetChurch;
using MyChurch.Application.Dtos;
using MediatR;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Entities;
using MyChurch.Application.Church.Commands.GenerateOnboardingQrCode;
using MyChurch.Application.Church.Queries.SearchPublicChurches; // added
using MyChurch.Application.Church.Commands;
using MyChurch.Application.Church.Queries.SearchNearby;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChurchController : BaseController
    {
        // Adiciona construtor para testes
        public ChurchController() : base() { }

        /// <summary>
        /// Busca pública de igrejas (não requer autenticação)
        /// Filtros avançados: rating mínimo, quantidade de reviews, ordenação por relevância/distância/rating
        /// </summary>
        /// <param name="query">Parâmetros de filtro e ordenação</param>
        /// <response code="200">Lista paginada com dados de avaliações, distância e relevância</response>
        [HttpGet("public/search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        public async Task<IActionResult> SearchPublicChurches([FromQuery] GetPublicChurchesQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Busca igrejas próximas por latitude/longitude e raio (km)
        /// </summary>
        [HttpGet("public/nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radiusKm = 5, [FromQuery] int? max = 50)
        {
            var result = await Mediator.Send(new GetNearbyChurchesQuery
            {
                Latitude = lat,
                Longitude = lng,
                RadiusKm = radiusKm,
                MaxResults = max
            });
            return Ok(result);
        }

        /// <summary>
        /// Create a new Church
        /// </summary>
        /// <response code="200">Success: Church Created</response>
        /// <response code="400">Failure: Invalid Request</response>
        /// <response code="401">Failure: Unauthorized</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateChurch(CreateChurchCommand command)
        {
            var mediator = await Mediator.Send(command);
            return Ok(mediator);
        }

        /// <summary>
        /// Update a Church
        /// </summary>
        /// <response code="200">Success: Church Updated</response>
        /// <response code="400">Failure: Invalid Request</response>
        /// <response code="401">Failure: Unauthorized</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChurchDto))]
        public async Task<IActionResult> UpdateChurch(int id, UpdateChurchCommand command)
        {
            command.Id = id;
            var mediator = await Mediator.Send(command);
            return Ok(mediator);
        }

        /// <summary>
        /// Cria uma nova Igreja já com usuário Admin
        /// </summary>
        /// <response code="200">Sucesso: Igreja criada</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPost("withadmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> CreateChurchWithAdmin([FromBody] CreateChurchWithAdminMemberCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Atualiza os dados bancários da igreja
        /// </summary>
        /// <response code="200">Sucesso: Dados bancários atualizados</response>
        /// <response code="400">Falha: Requisição inválida</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPut("banking-info")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankingInfoDto))]
        public async Task<IActionResult> UpdateBankingInfo([FromBody] UpdateBankingInfoCommand command)
        {
            // ChurchId será resolvido pelo membro logado no handler
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Dashboard com indicadores da igreja
        /// </summary>
        /// <response code="200">Sucesso: Dados consolidados</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChurchDashboard()
        {
            var command = AuthorizationRequestCreate<GetChurchDashboardQuery>();

            var result = await Mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Update a Church
        /// </summary>
        /// <response code="200">Success: Returned Church</response>
        /// <response code="400">Failure: Invalid Request</response>
        /// <response code="401">Failure: Unauthorized</response>
        [Authorize()]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChurchDto))]
        public async Task<IActionResult> GetChurch([FromRoute] int id)
        {
            var command = AuthorizationRequestCreate<GetChurchByIdQuery>();
                command.Id = id;
            var church = await Mediator.Send(command);
            return Ok(church);
        }

        /// <summary>
        /// Força a geração do QRCode de onboarding para a igreja caso não exista
        /// </summary>
        /// <param name="id">Id da igreja</param>
        /// <response code="200">QRCode base64</response>
        /// <response code="404">Igreja não encontrada</response>
        [HttpPost("{id}/generate-onboarding-qrcode")]
        public async Task<IActionResult> GenerateOnboardingQrCode(int id)
        {
            var qrCode = await Mediator.Send(new MyChurch.Application.Church.Commands.GenerateOnboardingQrCode.GenerateOnboardingQrCodeCommand(id));
            if (string.IsNullOrEmpty(qrCode))
                return NotFound("Igreja não encontrada");
            return Ok(new { qrCode });
        }

        /// <summary>
        /// Atualiza latitude e longitude da igreja usando o endereço cadastrado (Google Geocoding API)
        /// </summary>
        /// <param name="id">Id da igreja</param>
        /// <response code="200">Localização atualizada</response>
        /// <response code="404">Igreja não encontrada</response>
        [HttpPost("{id}/update-location")]
        public async Task<IActionResult> UpdateLocation(int id)
        {
            var result = await Mediator.Send(new UpdateChurchLocationCommand { ChurchId = id });
            if (!result) return NotFound();
            return Ok(new { success = true });
        }
    }
}