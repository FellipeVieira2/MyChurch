using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Visitors.Commands;
using MyChurch.Application.Donations.Commands;
using MyChurch.Application.Visitors.Queries;
using MyChurch.Domain.Enum;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitorController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateAnonymousVisitorCommand command)
        {
            var id = await Mediator.Send(command);
            return Ok(new { visitorId = id });
        }

        [HttpPost("donations")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDonation([FromBody] CreateVisitorDonationCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("status/update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateVisitorStatusCommand body)
        {
            var cmd = AuthorizationRequestCreate<UpdateVisitorStatusCommand>();
            cmd.VisitorId = body.VisitorId;
            cmd.NewStatus = body.NewStatus;
            cmd.Note = body.Note;
            var ok = await Mediator.Send(cmd);
            return Ok(new { success = ok });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVisitors([FromQuery] GetVisitorsQuery body)
        {
            var q = AuthorizationRequestCreate<GetVisitorsQuery>();
            q.Status = body.Status;
            q.MinScore = body.MinScore;
            q.DaysSinceLastVisitGreaterThan = body.DaysSinceLastVisitGreaterThan;
            q.NeedsFollowUp = body.NeedsFollowUp;
            q.Search = body.Search;
            q.Page = body.Page;
            q.PageSize = body.PageSize;
            var result = await Mediator.Send(q);
            return Ok(result);
        }

        [HttpGet("{visitorId}/timeline")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTimeline(int visitorId)
        {
            var q = AuthorizationRequestCreate<GetVisitorTimelineQuery>();
            q.VisitorId = visitorId;
            var result = await Mediator.Send(q);
            return Ok(result);
        }

        [HttpPost("promote")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Promote([FromBody] PromoteVisitorToMemberCommand body)
        {
            var cmd = AuthorizationRequestCreate<PromoteVisitorToMemberCommand>();
            cmd.VisitorId = body.VisitorId;
            cmd.Role = body.Role;
            cmd.Notes = body.Notes;
            var memberId = await Mediator.Send(cmd);
            return Ok(new { memberId });
        }
    }
}
