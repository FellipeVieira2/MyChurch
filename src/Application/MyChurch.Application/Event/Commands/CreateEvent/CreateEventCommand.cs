using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Application.Plans.Services;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.Event.Commands.CreateEvent
{
    public class CreateEventCommand : JwtMemberDto, IRequest<int>
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public DateTime FinishDate { get; set; }
        public string? Location { get; set; }
        public bool RequiresParticipantList { get; set; }

        public int? DepartmentId { get; set; }

        // Recorrência
        public EventRecurrenceType? RecurrenceType { get; set; }
        public int? Frequency { get; set; }

        // Tipo de evento
        public EventType EventType { get; set; } = EventType.General;

        // Tema do culto (opcional)
        public string? WorshipTheme { get; set; }
    }

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlanLimitService _planLimits;

        public CreateEventCommandHandler(IUnitOfWork unitOfWork, IPlanLimitService planLimits)
        {
            _unitOfWork = unitOfWork;
            _planLimits = planLimits;
        }

        public async Task<int> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            await _planLimits.EnsureMaxEventsAllowedAsync(churchId, additionalEventsToAdd: 1, cancellationToken);

            if (request.DepartmentId.HasValue)
            {
                var dept = await _unitOfWork.Departments.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.DepartmentId.Value && d.ChurchId == churchId, cancellationToken);

                if (dept == null)
                    ValidationException.ThrowException("Department", "Departamento não encontrado para esta igreja.");
            }

            var entity = new Domain.Entities.Event
            {
                Title = request.Title,
                Description = request.Description ?? "",
                Date = request.Date,
                FinishDate = request.FinishDate,
                Location = request.Location ?? "",
                ChurchId = churchId,
                RequiresParticipantList = request.RequiresParticipantList,
                EventType = request.EventType,
                DepartmentId = request.DepartmentId
            };

            await _unitOfWork.Events.Create(entity);

            if (request.RecurrenceType.HasValue && request.RecurrenceType != EventRecurrenceType.None && request.Frequency.HasValue)
            {
                var recurrence = new EventRecurrence
                {
                    Event = entity,
                    RecurrenceType = request.RecurrenceType.Value,
                    Frequency = request.Frequency.Value
                };
                await _unitOfWork.EventRecurrences.Create(recurrence);
            }

            await _unitOfWork.CommitAsync();

            if (request.EventType == EventType.WorshipService)
            {
                var worshipService = new Domain.Entities.WorshipService
                {
                    ChurchId = churchId,
                    Title = request.Title,
                    Theme = request.WorshipTheme,
                    StartTime = request.Date,
                    EndTime = request.FinishDate,
                    Description = request.Description ?? "",
                    EventId = entity.Id,
                    DepartmentId = request.DepartmentId
                };
                await _unitOfWork.WorshipServices.Create(worshipService);
                await _unitOfWork.CommitAsync();
            }

            return entity.Id;
        }
    }
}