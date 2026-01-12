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
        [JsonIgnore]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? FinishDate { get; set; }
        public string? Location { get; set; }
        public bool? RequiresParticipantList { get; set; }

        public int? DepartmentId { get; set; }

        // Recorrência
        public EventRecurrenceType? RecurrenceType { get; set; }
        public int? Frequency { get; set; }

        // Tema do culto (opcional, para atualização de WorshipService)
        public string? WorshipTheme { get; set; }
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
                .Include(e => e.WorshipServices)
                .Include(x => x.Recurrence)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ChurchId == churchId, cancellationToken);

            if (eventEntity == null)
                ValidationException.ThrowException("Event", "Event not found or does not belong to your church.");

            if (request.DepartmentId.HasValue)
            {
                var dept = await _unitOfWork.Departments.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.DepartmentId.Value && d.ChurchId == churchId, cancellationToken);

                if (dept == null)
                    ValidationException.ThrowException("Department", "Departamento não encontrado para esta igreja.");
            }

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

            if (request.DepartmentId.HasValue || request.DepartmentId == null)
                eventEntity.DepartmentId = request.DepartmentId;

            // Atualiza WorshipService(s) se existirem (um para cada ocorrência do evento)
            if (eventEntity.WorshipServices != null && eventEntity.WorshipServices.Any())
            {
                foreach (var worshipService in eventEntity.WorshipServices)
                {
                    if (request.Title != null)
                        worshipService.Title = request.Title;
                    if (request.Description != null)
                        worshipService.Description = request.Description;
                    if (request.WorshipTheme != null)
                        worshipService.Theme = request.WorshipTheme;
                    if (request.Date.HasValue)
                        worshipService.StartTime = request.Date.Value;
                    if (request.FinishDate.HasValue)
                        worshipService.EndTime = request.FinishDate.Value;

                    if (request.DepartmentId.HasValue || request.DepartmentId == null)
                        worshipService.DepartmentId = request.DepartmentId;
                }
            }

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