using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Commands.ManageKidsScale
{
    public class AddKidsScaleMemberCommand : JwtMemberDto, IRequest<int>
    {
        public int EventId { get; set; }
        public int MemberId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? Notes { get; set; }
    }

    public class AddKidsScaleMemberCommandHandler : IRequestHandler<AddKidsScaleMemberCommand, int>
    {
        private readonly IUnitOfWork _uow;

        public AddKidsScaleMemberCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> Handle(AddKidsScaleMemberCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "Usuário autenticado não encontrado.");

            if (actor.Role != UserRole.Admin)
                ValidationException.ThrowException("Auth", "Apenas administradores podem gerenciar a escala kids.");

            var ev = await _uow.Events.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.EventId && e.ChurchId == actor.ChurchId, cancellationToken);

            if (ev == null)
                ValidationException.ThrowException("Event", "Evento não encontrado ou não pertence à sua igreja.");

            var targetMember = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.MemberId && m.ChurchId == actor.ChurchId, cancellationToken);

            if (targetMember == null)
                ValidationException.ThrowException("Member", "Membro não encontrado para esta igreja.");

            if (ev.DepartmentId.HasValue)
            {
                var isDepartmentMember = await _uow.DepartmentMembers.Query()
                    .AsNoTracking()
                    .AnyAsync(dm => dm.DepartmentId == ev.DepartmentId.Value && dm.MemberId == request.MemberId && dm.IsActive, cancellationToken);

                if (!isDepartmentMember)
                    ValidationException.ThrowException("Member", "O membro selecionado não faz parte do ministério vinculado ao evento.");
            }

            var normalizedRoleName = request.RoleName?.Trim();
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                ValidationException.ThrowException("RoleName", "A função na escala é obrigatória.");

            var alreadyExists = await _uow.KidsScaleMembers.Query()
                .AsNoTracking()
                .AnyAsync(x => x.EventId == request.EventId
                    && x.MemberId == request.MemberId
                    && x.RoleName.ToLower() == normalizedRoleName.ToLower(), cancellationToken);

            if (alreadyExists)
                ValidationException.ThrowException("KidsScale", "Este membro já está escalado com esta função neste evento.");

            var item = new KidsScaleMember
            {
                EventId = request.EventId,
                MemberId = request.MemberId,
                RoleName = normalizedRoleName,
                Order = request.Order,
                Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
            };

            await _uow.KidsScaleMembers.Create(item);
            await _uow.CommitAsync();
            return item.Id;
        }
    }
}
