using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.PlatformUsers.Commands.DeletePlatformUser
{
    public class DeletePlatformUserCommand : PlatformUserJwtDto, IRequest<bool>
    {
        public int TargetUserId { get; set; }
    }

    public class DeletePlatformUserCommandHandler : IRequestHandler<DeletePlatformUserCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public DeletePlatformUserCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(DeletePlatformUserCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Id == request.PlatformUserId, cancellationToken);
            if (actor == null || !actor.IsActive || actor.Role != UserRole.PlatformAdmin)
                ValidationException.ThrowException("PlatformUser", "Only PlatformAdmin can delete platform users.");

            if (request.TargetUserId == actor.Id)
                ValidationException.ThrowException("PlatformUser", "You cannot delete yourself.");

            var target = await _uow.PlatformUsers.Query().FirstOrDefaultAsync(u => u.Id == request.TargetUserId, cancellationToken);
            if (target == null)
                ValidationException.ThrowException("PlatformUser", "User not found.");

            // Delete hard (table is platform-only). Alternatively make inactive.
            _uow.PlatformUsers.Delete(target);
            await _uow.CommitAsync();
            return true;
        }
    }
}
