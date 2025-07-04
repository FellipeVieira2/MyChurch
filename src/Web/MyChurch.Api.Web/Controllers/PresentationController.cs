using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Presentation.Commands;
using MyChurch.Application.Presentation.Queries;
using MyChurch.Application.Slide.Commands;
using MyChurch.Application.LivePresentation.Commands;
using MyChurch.Application.Dtos;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PresentationController : BaseController
    {
        private readonly IMediator _mediator;

        public PresentationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<PresentationDto>> Create([FromBody] CreatePresentationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<PresentationDto>>> GetAll()
        {
            var query = new GetAllPresentationsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PresentationDto>> GetById(int id)
        {
            var query = AuthorizationRequestCreate<GetPresentationByIdQuery>();
            query.Id = id;
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{presentationId}/slide")]
        public async Task<ActionResult<SlideDto>> AddSlide(int presentationId, [FromBody] AddSlideCommand command)
        {
            command.PresentationId = presentationId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("slide/{id}")]
        public async Task<IActionResult> UpdateSlide(int id, [FromBody] UpdateSlideCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("slide/{id}")]
        public async Task<IActionResult> DeleteSlide(int id)
        {
            var command = new DeleteSlideCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{presentationId}/live/start")]
        public async Task<IActionResult> StartLive(int presentationId, [FromBody] StartLivePresentationCommand command)
        {
            command.PresentationId = presentationId;
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("{presentationId}/live/end")]
        public async Task<IActionResult> EndLive(int presentationId, [FromBody] EndLivePresentationCommand command)
        {
            command.PresentationId = presentationId;
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("{presentationId}/live/next")]
        public async Task<IActionResult> NextSlide(int presentationId, [FromBody] GoToNextSlideCommand command)
        {
            command.PresentationId = presentationId;
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("{presentationId}/live/prev")]
        public async Task<IActionResult> PreviousSlide(int presentationId, [FromBody] GoToPreviousSlideCommand command)
        {
            command.PresentationId = presentationId;
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("{presentationId}/live/goto/{targetIndex}")]
        public async Task<IActionResult> GoToSlide(int presentationId, int targetIndex, [FromBody] GoToSlideByIndexCommand command)
        {
            command.PresentationId = presentationId;
            command.TargetIndex = targetIndex;
            await _mediator.Send(command);
            return Ok();
        }
    }
}
