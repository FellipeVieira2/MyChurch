using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Commands.ManageEventParticipants
{
    public class ManageEventParticipantsCommand : JwtMemberDto, IRequest<EventDto>
    {
        public int EventId { get; set; }
        public List<int> ParticipantIds { get; set; } = new();
    }

    public class ManageEventParticipantsCommandHandler : IRequestHandler<ManageEventParticipantsCommand, EventDto>
    {
        private readonly IUnitOfWork _uow;

        public ManageEventParticipantsCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EventDto> Handle(ManageEventParticipantsCommand request, CancellationToken cancellationToken)
        {
            var actor = await _uow.Members.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (actor == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            if (actor.Role != UserRole.Admin)
                ValidationException.ThrowException("Auth", "Only admins can manage event participants.");

            var ev = await _uow.Events.Query()
                .Include(e => e.Participants)
                .Include(e => e.Department)
                .Include(e => e.Church)
                .Include(e => e.Notifications)
                .Include(e => e.Recurrence)
                .FirstOrDefaultAsync(e => e.Id == request.EventId && e.ChurchId == actor.ChurchId, cancellationToken);

            if (ev == null)
                ValidationException.ThrowException("Event", "Event not found or does not belong to your church.");

            if (!ev.RequiresParticipantList)
                ValidationException.ThrowException("Event", "This event does not require participant list.");

            var ids = (request.ParticipantIds ?? new List<int>())
                .Distinct()
                .ToList();

            // valida membros pertencem à mesma igreja
            var validMembers = await _uow.Members.Query()
                .AsNoTracking()
                .Where(m => m.ChurchId == actor.ChurchId && ids.Contains(m.Id))
                .Select(m => m.Id)
                .ToListAsync(cancellationToken);

            var validSet = validMembers.ToHashSet();

            // Remove os que não estão mais na lista
            var toRemove = ev.Participants.Where(p => !validSet.Contains(p.Id)).ToList();
            foreach (var p in toRemove)
                ev.Participants.Remove(p);

            // Adiciona os novos
            var existing = ev.Participants.Select(p => p.Id).ToHashSet();
            var toAddIds = validMembers.Where(id => !existing.Contains(id)).ToList();
            if (toAddIds.Count > 0)
            {
                var toAdd = await _uow.Members.Query()
                    .Where(m => toAddIds.Contains(m.Id))
                    .ToListAsync(cancellationToken);

                foreach (var m in toAdd)
                    ev.Participants.Add(m);
            }

            await _uow.CommitAsync();

            return EventDto.New(ev);
        }
    }
}
