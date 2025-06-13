using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;

namespace MyChurch.Application.WorshipActivity.Commands
{
    public class FinalizeWorshipCommand : IRequest<bool>
    {
        public int WorshipServiceId { get; set; }
    }

    public class FinalizeWorshipCommandHandler : IRequestHandler<FinalizeWorshipCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinalizeWorshipCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(FinalizeWorshipCommand request, CancellationToken cancellationToken)
        {
            // Busca o culto
            var worshipService = await _unitOfWork.WorshipServices.Query()
                .Include(ws => ws.Event)
                    .ThenInclude(e => e.Recurrence)
                .FirstOrDefaultAsync(ws => ws.Id == request.WorshipServiceId, cancellationToken);

            if (worshipService == null)
                ValidationException.ThrowException("WorshipService", "Culto não encontrado.");

            // Finaliza o culto
            worshipService.Status = WorshipServiceStatus.Finished;
            worshipService.EndTime = DateTime.UtcNow;
            _unitOfWork.WorshipServices.Update(worshipService);

            // Verifica recorrência
            var recurrence = worshipService.Event?.Recurrence;
            if (recurrence != null && recurrence.RecurrenceType != EventRecurrenceType.None)
            {
                // Calcula próxima data
                var nextDate = GetNextOccurrence(worshipService.StartTime, recurrence.RecurrenceType, recurrence.Frequency);
                if (recurrence.RecurrenceEndDate == null || nextDate <= recurrence.RecurrenceEndDate)
                {
                    // Cria novo WorshipService para próxima ocorrência
                    var newWorship = new Domain.Entities.WorshipService
                    {
                        ChurchId = worshipService.ChurchId,
                        Title = worshipService.Title,
                        Theme = worshipService.Theme,
                        StartTime = nextDate,
                        EndTime = nextDate.Add(worshipService.EndTime.HasValue ? (worshipService.EndTime.Value - worshipService.StartTime) : TimeSpan.FromHours(1)),
                        Description = worshipService.Description,
                        EventId = worshipService.EventId,
                        Status = WorshipServiceStatus.NotStarted
                    };
                    _unitOfWork.WorshipServices.Create(newWorship);
                }
            }

            await _unitOfWork.CommitAsync();
            return true;
        }

        private DateTime GetNextOccurrence(DateTime current, EventRecurrenceType type, int frequency)
        {
            return type switch
            {
                EventRecurrenceType.Daily => current.AddDays(frequency),
                EventRecurrenceType.Weekly => current.AddDays(7 * frequency),
                EventRecurrenceType.Monthly => current.AddMonths(frequency),
                EventRecurrenceType.Yearly => current.AddYears(frequency),
                _ => current
            };
        }
    }
}