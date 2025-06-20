using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mychurch.Common.Utils.Objects;
using MyChurch.Application.Dtos;
using MyChurch.Application.Journey.Commands;
using MyChurch.Application.Journey.Queries;
using System.Threading.Tasks;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/journeys")]
    [Authorize]
    public class JourneyController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateJourney([FromBody] CreateJourneyCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(id);
        }

        [HttpPost("complete-stage/{id}")]
        public async Task<IActionResult> CompleteJourneyStage([FromRoute] int id, [FromBody] CompleteJourneyStageCommand command)
        {
            command.JourneyStageId = id;
            await Mediator.Send(command);
            return Ok();
        }

        [HttpPost("complete-daily-challenge/{id}")]
        public async Task<IActionResult> CompleteDailyChallenge([FromRoute] int id)
        {
            var cmd = AuthorizationRequestCreate<CompleteDailyChallengeCommand>();
            cmd.DailyChallengeId = id;
            await Mediator.Send(cmd);
            return Ok();
        }

        [HttpPost("generate-content")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GenerateJourneyContent([FromBody] GenerateJourneyContentCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("verify-stage")]
        [Authorize(Roles = "Admin,Leader")]
        public async Task<IActionResult> VerifyJourneyStage([FromBody] VerifyJourneyStageCommand command)
        {
            await Mediator.Send(command);
            return Ok();
        }

        [HttpGet("my-journeys")]
        public async Task<IActionResult> GetMyAssignedJourneys([FromQuery] GetMyAssignedJourneysQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery] GetLeaderboardQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("pastoral-alerts")]
        [Authorize(Roles = "Admin,Leader")]
        public async Task<IActionResult> GetPastoralAlerts()
        {
            var q = AuthorizationRequestCreate<GetPastoralAlertsQuery>();
            var result = await Mediator.Send(q);
            return Ok(result);
        }
    }
}
