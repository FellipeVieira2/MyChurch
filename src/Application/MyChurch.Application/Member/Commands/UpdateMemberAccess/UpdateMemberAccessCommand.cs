using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Member.Commands.UpdateMemberAccess
{
    public class UpdateMemberAccessCommand : JwtMemberDto, IRequest<MemberDto>
    {
        [JsonIgnore]
        public int MemberId { get; set; }

        public bool? IsActive { get; set; }

        public UserRole? Role { get; set; }
    }

    public class UpdateMemberAccessCommandHandler : IRequestHandler<UpdateMemberAccessCommand, MemberDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateMemberAccessCommandHandler> _logger;
        private readonly IPlanAccessService _planAccess;

        public UpdateMemberAccessCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateMemberAccessCommandHandler> logger, IPlanAccessService planAccess)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _planAccess = planAccess;
        }

        public async Task<MemberDto> Handle(UpdateMemberAccessCommand request, CancellationToken cancellationToken)
        {
            var actor = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Authenticated member does not exist.");

            var isPlatformAdmin = actor.Role == UserRole.PlatformAdmin;
            var isAdmin = actor.Role == UserRole.Admin;

            if (!isPlatformAdmin && !isAdmin)
                ValidationException.ThrowException("Member", "Only admins can update member access.");

            // PlatformAdmin pode operar globalmente; Admin fica restrito à própria igreja.
            var membersQuery = _unitOfWork.Members.Query()
                .Include(m => m.Documents)
                .Include(m => m.Address)
                .AsQueryable();

            if (!isPlatformAdmin)
                membersQuery = membersQuery.Where(m => m.ChurchId == actor.ChurchId);

            var member = await membersQuery
                .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken);

            if (member == null)
                ValidationException.ThrowException("Member", isPlatformAdmin
                    ? "Member not found."
                    : "Member not found or does not belong to your church.");

            // Não permitir desativar a própria conta via endpoint
            if (member.Id == actor.Id && request.IsActive is false)
                ValidationException.ThrowException("Member", "You cannot deactivate your own account.");

            if (request.IsActive.HasValue)
                member.IsActive = request.IsActive.Value;

            if (request.Role.HasValue)
            {
                var newRole = request.Role.Value;

                // SOMENTE PlatformAdmin pode promover para PlatformAdmin
                if (newRole == UserRole.PlatformAdmin && !isPlatformAdmin)
                    ValidationException.ThrowException("Member", "Only PlatformAdmin can grant PlatformAdmin role.");

                // Admin não pode alterar PlatformAdmin
                if (member.Role == UserRole.PlatformAdmin && !isPlatformAdmin)
                    ValidationException.ThrowException("Member", "Only PlatformAdmin can update a PlatformAdmin.");

                // Evitar rebaixar o último Admin da igreja
                if (member.Role == UserRole.Admin && newRole != UserRole.Admin)
                {
                    var otherAdminsCount = await _unitOfWork.Members.Query()
                        .CountAsync(m => m.ChurchId == member.ChurchId && m.Role == UserRole.Admin && m.Id != member.Id, cancellationToken);

                    if (otherAdminsCount == 0)
                        ValidationException.ThrowException("Member", "You cannot remove admin role from the last admin of the church.");
                }

                // Aplicar limites do plano apenas quando efetivamente está promovendo para um role limitado
                if (newRole != member.Role)
                    await _planAccess.EnsureRoleChangeAllowedAsync(member.ChurchId, newRole, cancellationToken);

                member.Role = newRole;
            }

            member.Updated = DateTime.UtcNow;

            _unitOfWork.Members.Update(member);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation(
                "Member access updated. ActorId: {ActorId}, ActorRole: {ActorRole}, MemberId: {MemberId}, IsActive: {IsActive}, Role: {Role}",
                actor.Id, actor.Role, member.Id, member.IsActive, member.Role);

            return MemberDto.New(member);
        }
    }
}
