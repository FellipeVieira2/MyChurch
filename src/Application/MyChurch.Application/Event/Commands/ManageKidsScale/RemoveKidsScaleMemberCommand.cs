using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Commands.ManageKidsScale
{
    public class RemoveKidsScaleMemberCommand : JwtMemberDto, IRequest<bool>
    {
        public int Id { get; set; }
        public int EventId { get; set; }
    }

    public class RemoveKidsScaleMemberCommandHandler : IRequestHandler<RemoveKidsScaleMemberCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public RemoveKidsScaleMemberCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(RemoveKidsScaleMemberCommand request, CancellationToken cancellationToken)
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

            var item = await _uow.KidsScaleMembers.Query()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.EventId == request.EventId, cancellationToken);

            if (item == null)
                ValidationException.ThrowException("KidsScale", "Item da escala kids não encontrado.");

            _uow.KidsScaleMembers.Delete(item);
            await _uow.CommitAsync();
            return true;
        }
    }
}
