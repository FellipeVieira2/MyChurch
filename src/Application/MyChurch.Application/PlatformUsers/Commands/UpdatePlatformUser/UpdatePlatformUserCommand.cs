using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.PlatformUsers.Commands.UpdatePlatformUser
{
    public class UpdatePlatformUserCommand : PlatformUserJwtDto, IRequest<bool>
    {
        public int TargetUserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Photo { get; set; }
        public UserRole? Role { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdatePlatformUserCommandHandler : IRequestHandler<UpdatePlatformUserCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlatformUserCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(UpdatePlatformUserCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Id == request.PlatformUserId, cancellationToken);
            if (actor == null || !actor.IsActive || actor.Role != UserRole.PlatformAdmin)
                ValidationException.ThrowException("PlatformUser", "Only PlatformAdmin can update platform users.");

            var target = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Id == request.TargetUserId, cancellationToken);
            if (target == null)
                ValidationException.ThrowException("PlatformUser", "User not found.");

            if (request.Name != null)
                target.Name = request.Name.Trim();

            if (request.Email != null)
            {
                var email = request.Email.Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(email))
                    ValidationException.ThrowException("Email", "Email is required.");

                var exists = await _uow.PlatformUsers.Query()
                    .AsNoTracking()
                    .AnyAsync(u => u.Id != target.Id && u.Email.ToLower() == email, cancellationToken);

                if (exists)
                    ValidationException.ThrowException("Email", "Email already exists.");

                target.Email = email;
            }

            if (request.Photo != null)
                target.Photo = request.Photo;

            if (request.Role.HasValue)
                target.Role = request.Role.Value;

            if (request.IsActive.HasValue)
                target.IsActive = request.IsActive.Value;

            target.Updated = DateTime.UtcNow;
            _uow.PlatformUsers.Update(target);
            await _uow.CommitAsync();
            return true;
        }
    }
}
