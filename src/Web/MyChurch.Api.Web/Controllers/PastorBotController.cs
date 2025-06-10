using Microsoft.AspNetCore.Mvc;
using MediatR;
//using MyChurch.Application.PastorBot.Commands.ExplainBiblePassage;
using MyChurch.Application.PastorBot.Commands.AskPastorBot;
using MyChurch.Application.PastorBot.Commands.VerseOfTheDay;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PastorBotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PastorBotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Ask to Pastor.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Explanation, context, and application in JSON format.</returns>
        [HttpPost("ask")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AskPastorBotResponse))]
        public async Task<IActionResult> Explain([FromBody] AskPastorBotCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Returns the verse of the day.
        /// </summary>
        /// <returns>Bible verse of the day in JSON format.</returns>
        [HttpGet("verseoftheday")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(VerseOfTheDayResponse))]
        public async Task<IActionResult> VerseOfTheDay()
        {
            var response = await _mediator.Send(new VerseOfTheDayCommand());
            return Ok(response);
        }
    }
}