using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateChurchCommand;

namespace MyChurch.Api.Web.Controllers
{
    public class ChurchController : BaseController
    {


        /// <summary>
        /// Create a new Church
        /// </summary>
        /// <response code="200">Success: Church Created</response>
        /// <response code="400">Failure: Invalid Requet</response>
        /// <response code="401">Failure: error</response>
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
        /// <response code="400">Failure: Invalid Requet</response>
        /// <response code="401">Failure: error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        public async Task<IActionResult> UpdateChurch(CreateChurchCommand command)
        {
            var mediator = await Mediator.Send(command);
            return Ok(mediator);
        }
    }
}
