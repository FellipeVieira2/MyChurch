using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateChurchCommand;
using MyChurch.Application.Church.Commands.UpdateChurch;
using MyChurch.Application.Dtos;

namespace MyChurch.Api.Web.Controllers
{
    public class ChurchController : BaseController
    {
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
    }
}