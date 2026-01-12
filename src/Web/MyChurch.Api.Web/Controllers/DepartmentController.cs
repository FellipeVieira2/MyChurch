using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Departments.Commands.CreateDepartment;
using MyChurch.Application.Departments.Queries.GetDepartments;
using MyChurch.Application.Departments.Commands.AddDepartmentMember;
using MyChurch.Application.Departments.Commands.RemoveDepartmentMember;
using MyChurch.Application.Departments.Queries.GetDepartmentMembers;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/departments")]
    [Authorize]
    public class DepartmentController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
        {
            var c = AuthorizationRequestCreate<CreateDepartmentCommand>();
            c.Name = command.Name;
            c.Description = command.Description;
            c.BankingInfoId = command.BankingInfoId;

            var id = await Mediator.Send(c);
            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetDepartmentsQuery query)
        {
            var q = AuthorizationRequestCreate<GetDepartmentsQuery>();
            q.PageNumber = query.PageNumber;
            q.PageSize = query.PageSize;
            q.IsActive = query.IsActive;
            q.Name = query.Name;

            var result = await Mediator.Send(q);
            return Ok(result);
        }

        [HttpGet("{departmentId}/members")]
        public async Task<IActionResult> GetMembers([FromRoute] int departmentId)
        {
            var q = AuthorizationRequestCreate<GetDepartmentMembersQuery>();
            q.DepartmentId = departmentId;
            var result = await Mediator.Send(q);
            return Ok(result);
        }

        [HttpPost("{departmentId}/members")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddMember([FromRoute] int departmentId, [FromBody] AddDepartmentMemberCommand command)
        {
            var c = AuthorizationRequestCreate<AddDepartmentMemberCommand>();
            c.DepartmentId = departmentId;
            c.MemberId = command.MemberId;
            c.Role = command.Role;

            var id = await Mediator.Send(c);
            return Ok(id);
        }

        [HttpDelete("{departmentId}/members/{memberId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveMember([FromRoute] int departmentId, [FromRoute] int memberId)
        {
            var c = AuthorizationRequestCreate<RemoveDepartmentMemberCommand>();
            c.DepartmentId = departmentId;
            c.MemberId = memberId;

            await Mediator.Send(c);
            return NoContent();
        }
    }
}
