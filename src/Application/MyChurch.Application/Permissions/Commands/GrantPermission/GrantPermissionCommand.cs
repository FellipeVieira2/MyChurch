using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using MyChurch.Domain.Services;

namespace MyChurch.Application.Permissions.Commands.GrantPermission
{
    public class GrantPermissionCommand : JwtMemberDto, IRequest
    {
        /// <summary>
        /// ID do membro que receberá a permissão
        /// </summary>
        public int TargetMemberId { get; set; }

        /// <summary>
        /// Permissão a ser concedida
        /// </summary>
        public Permission Permission { get; set; }

        /// <summary>
        /// Motivo da concessão (opcional)
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Data de expiração (null = permanente)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }

    public class GrantPermissionCommandHandler : IRequestHandler<GrantPermissionCommand>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPermissionService _permissionService;
        private readonly IPlanLimitService _planLimits;

        public GrantPermissionCommandHandler(IUnitOfWork uow, IPermissionService permissionService, IPlanLimitService planLimits)
        {
            _uow = uow;
            _permissionService = permissionService;
            _planLimits = planLimits;
        }

        public async Task Handle(GrantPermissionCommand request, CancellationToken cancellationToken)
        {
            var member = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", "Usuário não encontrado.");

            if (member.Role != UserRole.Admin)
                ValidationException.ThrowException("Permission", "Apenas Admin pode conceder permissões.");

            await _planLimits.EnsureAdvancedPermissionsAllowedAsync(member.ChurchId, cancellationToken);

            await _permissionService.GrantPermissionAsync(
                memberId: request.TargetMemberId,
                permission: request.Permission,
                grantedByMemberId: request.UserId,
                reason: request.Reason,
                expiresAt: request.ExpiresAt,
                cancellationToken: cancellationToken);
        }
    }
}
