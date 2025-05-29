using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace MyChurch.Application.Event.Commands.UpdateEvent
{
    public class UpdateEventCommand : JwtMemberDto, IRequest<int>
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? FinishDate { get; set; }
        public string? Location { get; set; }
        public bool? RequiresParticipantList { get; set; }

        // Recorrência
        public EventRecurrenceType? RecurrenceType { get; set; }
        public int? Frequency { get; set; }
    }

    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateEventCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            // Busca o membro logado para obter o ChurchId
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember is null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            // Busca o evento e valida se pertence à igreja do usuário
            var eventEntity = await _unitOfWork.Events.Query()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ChurchId == churchId, cancellationToken);

            if (eventEntity == null)
                ValidationException.ThrowException("Event", "Event not found or does not belong to your church.");

            if (request.Title != null)
                eventEntity.Title = request.Title;
            if (request.Description != null)
                eventEntity.Description = request.Description;
            if (request.Date.HasValue)
                eventEntity.Date = request.Date.Value;
            if (request.FinishDate.HasValue)
                eventEntity.FinishDate = request.FinishDate.Value;
            if (request.Location != null)
                eventEntity.Location = request.Location;
            if (request.RequiresParticipantList.HasValue)
                eventEntity.RequiresParticipantList = request.RequiresParticipantList.Value;

            // Recorrência
            if (request.RecurrenceType.HasValue && request.RecurrenceType != EventRecurrenceType.None && request.Frequency.HasValue)
            {
                if (eventEntity.Recurrence == null)
                {
                    eventEntity.Recurrence = new EventRecurrence
                    {
                        EventId = eventEntity.Id,
                        RecurrenceType = request.RecurrenceType.Value,
                        Frequency = request.Frequency.Value
                    };
                }
                else
                {
                    eventEntity.Recurrence.RecurrenceType = request.RecurrenceType.Value;
                    eventEntity.Recurrence.Frequency = request.Frequency.Value;
                }
            }
            else if (request.RecurrenceType.HasValue && request.RecurrenceType == EventRecurrenceType.None)
            {
                if (eventEntity.Recurrence != null)
                {
                    _unitOfWork.EventRecurrences.Delete(eventEntity.Recurrence);
                    eventEntity.Recurrence = null;
                }
            }

            await _unitOfWork.CommitAsync();
            return eventEntity.Id;
        }
    }
}
