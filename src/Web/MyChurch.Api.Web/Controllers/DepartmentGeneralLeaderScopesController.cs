using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Departments.Commands.GrantGeneralLeaderScope;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/church")]
    [Authorize(Roles = "Admin")]
    public class DepartmentGeneralLeaderScopesController : BaseController
    {
        public class GrantScopeRequest
        {
            public int BranchChurchId { get; set; }
            public int DepartmentId { get; set; }
            public int LeaderMemberId { get; set; }
            public bool IsActive { get; set; } = true;
        }

        // POST /api/church/{parentChurchId}/department-general-leader-scopes
        [HttpPost("{parentChurchId:int}/department-general-leader-scopes")]
        public async Task<IActionResult> GrantScope([FromRoute] int parentChurchId, [FromBody] GrantScopeRequest body)
        {
            var cmd = AuthorizationRequestCreate<GrantGeneralLeaderScopeCommand>();
            cmd.ParentChurchId = parentChurchId;
            cmd.BranchChurchId = body.BranchChurchId;
            cmd.DepartmentId = body.DepartmentId;
            cmd.LeaderMemberId = body.LeaderMemberId;
            cmd.IsActive = body.IsActive;

            var id = await Mediator.Send(cmd);
            return Ok(id);
        }
    }
}
