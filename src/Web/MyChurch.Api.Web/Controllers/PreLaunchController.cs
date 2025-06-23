using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.PreLaunch.Commands.ConfirmPreLaunchInterest;
using MyChurch.Application.PreLaunch.Commands.CreatePreLaunchInterest;
using MyChurch.Application.PreLaunch.Queries.GetPreLaunchInterests;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PreLaunchController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PreLaunchController> _logger;

        public PreLaunchController(IMediator mediator, ILogger<PreLaunchController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo interesse no pré-lançamento
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterInterest([FromBody] CreatePreLaunchInterestDto request)
        {
            _logger.LogInformation("Registering pre-launch interest for: {Email}", request.Email);

            var command = new CreatePreLaunchInterestCommand
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                ChurchName = request.ChurchName,
                ChurchRole = request.ChurchRole,
                Comments = request.Comments
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Confirma o email de um interesse no pré-lançamento através do token
        /// </summary>
        [HttpGet("confirm")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmInterest([FromQuery] string token)
        {
            _logger.LogInformation("Confirming pre-launch interest with token: {Token}", token);

            var command = new ConfirmPreLaunchInterestCommand
            {
                Token = token
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Obtém todos os interesses no pré-lançamento (requer autenticação de administrador)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(List<PreLaunchInterestDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInterests([FromQuery] bool? onlyConfirmed = null)
        {
            _logger.LogInformation("Getting pre-launch interests. OnlyConfirmed: {OnlyConfirmed}", onlyConfirmed);

            var query = new GetPreLaunchInterestsQuery
            {
                OnlyConfirmed = onlyConfirmed
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}