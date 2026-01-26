using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Departments.Queries.GetMyDepartmentSupervisionScopes;
using MyChurch.Application.Departments.Queries.GetDepartmentsCrossChurch;
using MyChurch.Application.Departments.Queries.GetDepartmentMembersCrossChurch;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/department-supervision")]
    [Authorize]
    public class DepartmentSupervisionController : BaseController
    {
        // GET /api/department-supervision/my-scopes
        [HttpGet("my-scopes")]
        public async Task<IActionResult> GetMyScopes()
        {
            var q = AuthorizationRequestCreate<GetMyDepartmentSupervisionScopesQuery>();
            var result = await Mediator.Send(q);
            return Ok(result);
        }

        // GET /api/department-supervision/parent/{parentChurchId}/branch/{branchChurchId}/departments
        [HttpGet("parent/{parentChurchId:int}/branch/{branchChurchId:int}/departments")]
        public async Task<IActionResult> GetDepartmentsCrossChurch(
            [FromRoute] int parentChurchId,
            [FromRoute] int branchChurchId)
        {
            var q = AuthorizationRequestCreate<GetDepartmentsCrossChurchQuery>();
            q.ParentChurchId = parentChurchId;
            q.BranchChurchId = branchChurchId;

            var result = await Mediator.Send(q);
            return Ok(result);
        }

        // GET /api/department-supervision/parent/{parentChurchId}/branch/{branchChurchId}/departments/{departmentId}/members
        [HttpGet("parent/{parentChurchId:int}/branch/{branchChurchId:int}/departments/{departmentId:int}/members")]
        public async Task<IActionResult> GetDepartmentMembersCrossChurch(
            [FromRoute] int parentChurchId,
            [FromRoute] int branchChurchId,
            [FromRoute] int departmentId,
            [FromQuery] bool onlyLeaders = false)
        {
            var q = AuthorizationRequestCreate<GetDepartmentMembersCrossChurchQuery>();
            q.ParentChurchId = parentChurchId;
            q.BranchChurchId = branchChurchId;
            q.DepartmentId = departmentId;
            q.OnlyLeaders = onlyLeaders;

            var result = await Mediator.Send(q);
            return Ok(result);
        }
    }
}
