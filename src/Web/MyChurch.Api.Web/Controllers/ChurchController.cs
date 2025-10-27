using QRCoder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
using MyChurch.Application.Church.Commands.UpdateChurchSocialMedia;
using MyChurch.Application.Church.Commands.UpdateChurchCharacteristics;
using MyChurch.Application.Church.Commands.UpdateChurchCapacity;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChurchController : BaseController
    {
        // Adiciona construtor para testes
        public ChurchController() : base() { }

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
        /// Atualiza as redes sociais e contatos da igreja (Website, Instagram, Facebook, YouTube, WhatsApp, Twitter, TikTok)
        /// </summary>
        /// <response code="200">Sucesso: Redes sociais atualizadas</response>
        /// <response code="400">Falha: URLs inválidas</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPut("social-media")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSocialMedia([FromBody] UpdateChurchSocialMediaCommand command)
        {
            await Mediator.Send(command);
            return Ok(new { message = "Redes sociais atualizadas com sucesso" });
        }

        /// <summary>
        /// Atualiza características avançadas da igreja (denominação, amenidades, idiomas) para busca avançada
        /// </summary>
        /// <response code="200">Sucesso: Características atualizadas</response>
        /// <response code="400">Falha: Dados inválidos</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPut("characteristics")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCharacteristics([FromBody] UpdateChurchCharacteristicsCommand command)
        {
            await Mediator.Send(command);
            return Ok(new { message = "Características da igreja atualizadas com sucesso" });
        }

        /// <summary>
        /// Atualiza capacidade e infraestrutura da igreja (lotação, estacionamento, equipamentos, instalações)
        /// </summary>
        /// <response code="200">Sucesso: Capacidade e infraestrutura atualizadas</response>
        /// <response code="400">Falha: Dados inválidos (capacidade negativa)</response>
        /// <response code="401">Falha: Não autorizado</response>
        [HttpPut("capacity")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCapacity([FromBody] UpdateChurchCapacityCommand command)
        {
            await Mediator.Send(command);
            return Ok(new { message = "Capacidade e infraestrutura atualizadas com sucesso" });
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
        /// Atualiza latitude e longitude da igreja.
        /// Pode ser atualizado manualmente (lat/lng) ou automaticamente via geocoding do endereço.
        /// Limite: 20 requests por minuto.
        /// </summary>
        /// <param name="id">Id da igreja</param>
        /// <param name="request">Dados de localização</param>
        /// <response code="200">Localização atualizada com sucesso</response>
        /// <response code="400">Coordenadas inválidas ou erro no geocoding</response>
        /// <response code="404">Igreja não encontrada</response>
        /// <response code="429">Muitas requisições - aguarde antes de tentar novamente</response>
        [HttpPut("{id}/location")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("upload-limiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateChurchLocationResult))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateChurchLocationRequest request)
        {
            var command = new UpdateChurchLocationCommand
            {
                ChurchId = id,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                AutoGeocode = request.AutoGeocode
            };

            var result = await Mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Atualiza localização da igreja automaticamente usando Google Geocoding (apenas via endereço).
        /// Limite: 20 requests por minuto.
        /// </summary>
        /// <param name="id">Id da igreja</param>
        /// <response code="200">Localização geocodificada com sucesso</response>
        /// <response code="404">Igreja não encontrada ou endereço inválido</response>
        /// <response code="429">Muitas requisições - aguarde antes de tentar novamente</response>
        [HttpPost("{id}/geocode")]
        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("upload-limiter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateChurchLocationResult))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GeocodeLocation(int id)
        {
            var command = new UpdateChurchLocationCommand
            {
                ChurchId = id,
                AutoGeocode = true
            };

            var result = await Mediator.Send(command);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }

    public class UpdateChurchLocationRequest
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool AutoGeocode { get; set; } = false;
    }
}