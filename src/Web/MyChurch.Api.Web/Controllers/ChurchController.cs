using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using MyChurch.Application.Church.Commands.CreateChurchCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
    }
}
