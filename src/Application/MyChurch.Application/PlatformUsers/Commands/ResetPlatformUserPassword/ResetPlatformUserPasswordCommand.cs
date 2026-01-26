using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.PlatformUsers.Commands.ResetPlatformUserPassword
{
    public class ResetPlatformUserPasswordCommand : PlatformUserJwtDto, IRequest<bool>
    {
        public int TargetUserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResetPlatformUserPasswordCommandHandler : IRequestHandler<ResetPlatformUserPasswordCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;

        public ResetPlatformUserPasswordCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> Handle(ResetPlatformUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Id == request.PlatformUserId, cancellationToken);
            if (actor == null || !actor.IsActive || actor.Role != UserRole.PlatformAdmin)
                ValidationException.ThrowException("PlatformUser", "Only PlatformAdmin can reset passwords.");

            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
                ValidationException.ThrowException("Password", "Password must be at least 6 characters.");

            var target = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Id == request.TargetUserId, cancellationToken);
            if (target == null)
                ValidationException.ThrowException("PlatformUser", "User not found.");

            target.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            target.Updated = DateTime.UtcNow;
            _uow.PlatformUsers.Update(target);
            await _uow.CommitAsync();
            return true;
        }
    }
}
