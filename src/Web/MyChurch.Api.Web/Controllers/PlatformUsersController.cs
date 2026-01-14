using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;

namespace MyChurch.Api.Web.Controllers
{
    [ApiController]
    [Route("api/platform/users")]
    [Authorize(Roles = "PlatformAdmin")]
    public class PlatformUsersController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public PlatformUsersController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] string? search = null)
        {
            var q = _uow.PlatformUsers.Query().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();
                q = q.Where(u => u.Email.ToLower().Contains(s) || u.Name.ToLower().Contains(s));
            }

            var users = await q.OrderByDescending(u => u.Created)
                .Take(200)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Photo,
                    Role = u.Role.ToString(),
                    u.IsActive,
                    u.Created,
                    u.Updated
                })
                .ToListAsync();

            return Ok(users);
        }

        public class UpdatePlatformUserAccessRequest
        {
            public bool? IsActive { get; set; }
        }

        [HttpPatch("{id:int}/access")]
        public async Task<IActionResult> UpdateAccess([FromRoute] int id, [FromBody] UpdatePlatformUserAccessRequest body)
        {
            var user = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            if (body.IsActive.HasValue)
                user.IsActive = body.IsActive.Value;

            user.Updated = DateTime.UtcNow;
            _uow.PlatformUsers.Update(user);
            await _uow.CommitAsync();

            return Ok(new
            {
                user.Id,
                user.IsActive,
                Role = user.Role.ToString()
            });
        }
    }
}
