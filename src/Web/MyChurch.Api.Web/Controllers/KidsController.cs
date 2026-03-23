using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Dtos;
using MyChurch.Application.Kids.Commands;
using MyChurch.Application.Kids.Queries;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KidsController : BaseController
    {
        [HttpGet("children")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<KidsChildDto>))]
        public async Task<IActionResult> GetMyChildren()
        {
            var query = AuthorizationRequestCreate<GetMyChildrenQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("children")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> RegisterChild([FromBody] RegisterChildCommand command)
        {
            var request = AuthorizationRequestCreate<RegisterChildCommand>();
            request.FullName = command.FullName;
            request.BirthDate = command.BirthDate;
            request.Gender = command.Gender;
            request.AuthorizedPickups = command.AuthorizedPickups;

            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("check-in")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KidsCheckInDto))]
        public async Task<IActionResult> CreateCheckIn([FromBody] CreateKidsCheckInCommand command)
        {
            var request = AuthorizationRequestCreate<CreateKidsCheckInCommand>();
            request.ChildId = command.ChildId;
            request.EnvironmentName = command.EnvironmentName;
            request.Notes = command.Notes;

            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("check-ins/active")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<KidsCheckInDto>))]
        public async Task<IActionResult> GetActiveCheckIns()
        {
            var query = AuthorizationRequestCreate<GetActiveKidsCheckInsQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("pickup/validate")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KidsPickupValidationDto))]
        public async Task<IActionResult> ValidatePickup([FromBody] ValidateKidsPickupQuery query)
        {
            var request = AuthorizationRequestCreate<ValidateKidsPickupQuery>();
            request.PickupToken = query.PickupToken;

            var result = await Mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("checkout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<IActionResult> CompleteCheckout([FromBody] CompleteKidsCheckoutCommand command)
        {
            var request = AuthorizationRequestCreate<CompleteKidsCheckoutCommand>();
            request.CheckInId = command.CheckInId;
            request.AuthorizedPickupId = command.AuthorizedPickupId;
            request.PickupToken = command.PickupToken;

            var result = await Mediator.Send(request);
            return Ok(result);
        }
    }
}
