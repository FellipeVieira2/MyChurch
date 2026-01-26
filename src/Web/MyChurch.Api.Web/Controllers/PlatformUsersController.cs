using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.PlatformUsers.Commands.CreatePlatformUser;
using MyChurch.Application.PlatformUsers.Commands.DeletePlatformUser;
using MyChurch.Application.PlatformUsers.Commands.ResetPlatformUserPassword;
using MyChurch.Application.PlatformUsers.Commands.UpdatePlatformUser;
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
        private readonly IMediator _mediator;

        public PlatformUsersController(IUnitOfWork uow, IMediator mediator)
        {
            _uow = uow;
            _mediator = mediator;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlatformUserCommand command)
        {
            if (HttpContext?.Items["PlatformUser"] is not PlatformUserJwtDto platformJwt)
                return Unauthorized();

            command.PlatformUserId = platformJwt.PlatformUserId;
            command.Email = command.Email?.Trim() ?? string.Empty;

            var id = await _mediator.Send(command);
            return Ok(id);
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

        public class ResetPasswordRequest
        {
            public string NewPassword { get; set; } = string.Empty;
        }

        [HttpPost("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword([FromRoute] int id, [FromBody] ResetPasswordRequest body)
        {
            if (HttpContext?.Items["PlatformUser"] is not PlatformUserJwtDto platformJwt)
                return Unauthorized();

            var cmd = new ResetPlatformUserPasswordCommand
            {
                PlatformUserId = platformJwt.PlatformUserId,
                TargetUserId = id,
                NewPassword = body.NewPassword
            };

            var ok = await _mediator.Send(cmd);
            return Ok(new { success = ok });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var user = await _uow.PlatformUsers.Query().AsNoTracking()
                .Where(u => u.Id == id)
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
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        public class UpdatePlatformUserRequest
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Photo { get; set; }
            public string? Role { get; set; }
            public bool? IsActive { get; set; }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePlatformUserRequest body)
        {
            if (HttpContext?.Items["PlatformUser"] is not PlatformUserJwtDto platformJwt)
                return Unauthorized();

            UserRole? role = null;
            if (!string.IsNullOrWhiteSpace(body.Role))
            {
                if (!Enum.TryParse<UserRole>(body.Role, ignoreCase: true, out var parsed))
                    return BadRequest(new { error = "invalid_role" });

                role = parsed;
            }

            var cmd = new UpdatePlatformUserCommand
            {
                PlatformUserId = platformJwt.PlatformUserId,
                TargetUserId = id,
                Name = body.Name,
                Email = body.Email,
                Photo = body.Photo,
                Role = role,
                IsActive = body.IsActive
            };

            var ok = await _mediator.Send(cmd);
            return Ok(new { success = ok });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (HttpContext?.Items["PlatformUser"] is not PlatformUserJwtDto platformJwt)
                return Unauthorized();

            var cmd = new DeletePlatformUserCommand
            {
                PlatformUserId = platformJwt.PlatformUserId,
                TargetUserId = id
            };

            var ok = await _mediator.Send(cmd);
            return Ok(new { success = ok });
        }
    }
}
