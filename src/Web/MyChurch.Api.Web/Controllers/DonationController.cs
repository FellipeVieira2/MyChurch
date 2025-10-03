using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Donation.Commands.CreateDonation;
using MyChurch.Application.Donation.Commands.TransferChurchBalance;
using MyChurch.Application.Donation.Queries.GetAllPaidDonations;
using MyChurch.Application.Donation.Queries.GetChurchTransferBalance;
namespace MyChurch.Api.Web.Controllers
{
    public class DonationController : BaseController
    {
        /// <summary>
        /// Create a new Donation
        /// </summary>
        /// <response code="200">Success: Donation Created</response>
        /// <response code="400">Failure: Invalid Request</response>
        /// <response code="401">Failure: Unauthorized</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDonation(CreateDonationCommand command)
        {
            var mediator = await Mediator.Send(command);
            return Ok(mediator);
        }

        /// <summary>
        /// Get all paid donations for the logged member (paginated)
        /// </summary>
        /// <response code="200">Success: List of paid donations</response>
        /// <response code="401">Failure: Unauthorized</response>
        [HttpGet("paid")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPaidDonations([FromQuery] GetAllPaidDonationsQuery query)
        {
            // Garante que o UserId do membro logado será usado
            var request = AuthorizationRequestCreate<GetAllPaidDonationsQuery>();
            request.Description = query.Description;
            request.Value = query.Value;
            request.Date = query.Date;
            request.Page = query.Page;
            request.PageSize = query.PageSize;

            var result = await Mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Valor disponível para repasse para a igreja
        /// </summary>
        [HttpGet("transfer-balance")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransferBalance()
        {
            var query = AuthorizationRequestCreate<GetChurchTransferBalanceQuery>();
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Efetua retirada do valor disponível para a conta da igreja
        /// </summary>
        [HttpPost("transfer")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> TransferChurchBalance([FromBody] TransferChurchBalanceCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
