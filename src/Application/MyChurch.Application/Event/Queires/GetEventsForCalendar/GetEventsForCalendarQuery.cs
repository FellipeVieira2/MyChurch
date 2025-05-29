using MediatR;
using Microsoft.EntityFrameworkCore;
using MyChurch.Application.Dtos;
using MyChurch.Domain.Contracts;
using MyChurch.Domain.Enum;
using MyChurch.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static MyChurch.Application.Dtos.EventCalendarDto;

namespace MyChurch.Application.Event.Queires.GetEventsForCalendar
{
    public class GetEventsForCalendarQuery : JwtMemberDto, IRequest<List<EventCalendarDto>>
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class GetEventsForCalendarQueryHandler : IRequestHandler<GetEventsForCalendarQuery, List<EventCalendarDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventsForCalendarQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EventCalendarDto>> Handle(GetEventsForCalendarQuery request, CancellationToken cancellationToken)
        {
            var loggedMember = await _unitOfWork.Members.Query()
                .FirstOrDefaultAsync(m => m.Id == request.UserId, cancellationToken);

            if (loggedMember == null)
                ValidationException.ThrowException("Member", "This Member does not exist.");

            int churchId = loggedMember.ChurchId;

            // Busca todos os eventos da igreja
            var events = await _unitOfWork.Events.Query()
                .Include(e => e.Recurrence)
                .Where(e => e.ChurchId == churchId)
                .ToListAsync(cancellationToken);

            var result = new List<EventCalendarDto>();

            foreach (var ev in events)
            {
                List<EventOccurrenceDto> occurrences = new();

                if (ev.Recurrence == null || ev.Recurrence.RecurrenceType == EventRecurrenceType.None)
                {
                    // Evento único: verifica se está no mês/ano solicitado
                    if (ev.Date.Year == request.Year && ev.Date.Month == request.Month)
                        occurrences.Add(new EventOccurrenceDto { Start = ev.Date, End = ev.FinishDate });
                }
                else
                {
                    // Evento recorrente: gera as datas do mês/ano solicitado, respeitando RecurrenceEndDate
                    occurrences = GenerateOccurrencesForMonth(
                        ev.Date,
                        ev.FinishDate,
                        ev.Recurrence.RecurrenceType,
                        ev.Recurrence.Frequency,
                        ev.Recurrence.RecurrenceEndDate,
                        request.Year,
                        request.Month
                    );
                }

                if (occurrences.Any())
                    result.Add(EventCalendarDto.New(ev, occurrences));
            }

            return result;
        }

        // Gera as ocorrências (início e fim) do evento para o mês/ano solicitado, respeitando RecurrenceEndDate
        private static List<EventOccurrenceDto> GenerateOccurrencesForMonth(
            DateTime startDate,
            DateTime finishDate,
            EventRecurrenceType recurrenceType,
            int frequency,
            DateTime? recurrenceEndDate,
            int year,
            int month)
        {
            var occurrences = new List<EventOccurrenceDto>();
            var currentStart = startDate;
            var duration = finishDate - startDate;

            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // Define o limite final: o menor entre o fim do mês e o RecurrenceEndDate (se houver)
            DateTime? endLimit = recurrenceEndDate.HasValue && recurrenceEndDate.Value < monthEnd
                ? recurrenceEndDate.Value
                : monthEnd;

            while (currentStart <= endLimit)
            {
                if (currentStart >= monthStart && currentStart <= monthEnd)
                {
                    occurrences.Add(new EventOccurrenceDto
                    {
                        Start = currentStart,
                        End = currentStart.Add(duration)
                    });
                }

                currentStart = recurrenceType switch
                {
                    EventRecurrenceType.Daily => currentStart.AddDays(frequency),
                    EventRecurrenceType.Weekly => currentStart.AddDays(7 * frequency),
                    EventRecurrenceType.Monthly => currentStart.AddMonths(frequency),
                    EventRecurrenceType.Yearly => currentStart.AddYears(frequency),
                    _ => currentStart
                };

                if (frequency <= 0) break;
            }

            return occurrences;
        }
    }
}
