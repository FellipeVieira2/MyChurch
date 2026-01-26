using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyChurch.Application.Church.Commands.CreateBranch;
using MyChurch.Application.Church.Queries.ListBranches;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/church")]
    public class ChurchBranchesController : BaseController
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/branches")]
        public async Task<IActionResult> ListBranches([FromRoute] int id)
        {
            var q = AuthorizationRequestCreate<ListBranchesQuery>();
            q.ParentChurchId = id;
            var result = await Mediator.Send(q);
            return Ok(result);
        }

        public class AddressRequest
        {
            public string Street { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string State { get; set; } = string.Empty;
            public string ZipCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string Neighborhood { get; set; } = string.Empty;
            public string Number { get; set; } = string.Empty;
        }

        public class CreateBranchRequest
        {
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string Phone { get; set; } = string.Empty;
            public string? Document { get; set; }

            public AddressRequest Address { get; set; } = null!;

            public string AdminName { get; set; } = string.Empty;
            public string? AdminEmail { get; set; }
            public string AdminPhone { get; set; } = string.Empty;
            public DateTime AdminBirthDate { get; set; }
            public bool AdminIsBaptized { get; set; }
            public DateTime? AdminBaptizedDate { get; set; }
            public bool AdminIsTither { get; set; }
            public string AdminPassword { get; set; } = string.Empty;
            public string? AdminBirthCity { get; set; }
            public string? AdminBirthState { get; set; }
            public string? Ministry { get; set; }
            public DateTime MemberSince { get; set; }
            public string? Notes { get; set; }
            public AddressRequest AdminAddress { get; set; } = null!;
            public MyChurch.Domain.Enum.MaritalStatus? MaritalStatus { get; set; }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id:int}/branches")]
        public async Task<IActionResult> CreateBranch([FromRoute] int id, [FromBody] CreateBranchRequest body)
        {
            var cmd = AuthorizationRequestCreate<CreateBranchCommand>();
            cmd.ParentChurchId = id;
            cmd.Name = body.Name;
            cmd.Description = body.Description;
            cmd.Phone = body.Phone;
            cmd.Document = body.Document;
            cmd.Address = new CreateBranchCommand.AddressDto
            {
                Street = body.Address.Street,
                City = body.Address.City,
                State = body.Address.State,
                ZipCode = body.Address.ZipCode,
                Country = body.Address.Country,
                Neighborhood = body.Address.Neighborhood,
                Number = body.Address.Number
            };

            cmd.AdminName = body.AdminName;
            cmd.AdminEmail = body.AdminEmail;
            cmd.AdminPhone = body.AdminPhone;
            cmd.AdminBirthDate = body.AdminBirthDate;
            cmd.AdminIsBaptized = body.AdminIsBaptized;
            cmd.AdminBaptizedDate = body.AdminBaptizedDate;
            cmd.AdminIsTither = body.AdminIsTither;
            cmd.AdminPassword = body.AdminPassword;
            cmd.AdminBirthCity = body.AdminBirthCity;
            cmd.AdminBirthState = body.AdminBirthState;
            cmd.Ministry = body.Ministry;
            cmd.MemberSince = body.MemberSince;
            cmd.Notes = body.Notes;
            cmd.AdminAddress = new CreateBranchCommand.AddressDto
            {
                Street = body.AdminAddress.Street,
                City = body.AdminAddress.City,
                State = body.AdminAddress.State,
                ZipCode = body.AdminAddress.ZipCode,
                Country = body.AdminAddress.Country,
                Neighborhood = body.AdminAddress.Neighborhood,
                Number = body.AdminAddress.Number
            };
            cmd.MaritalStatus = body.MaritalStatus;

            var result = await Mediator.Send(cmd);
            return Ok(result);
        }
    }
}
