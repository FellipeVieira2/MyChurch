using System;
using System.Collections.Generic;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class EventCalendarDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int ChurchId { get; set; }
        public bool IsRecurring { get; set; }
        public EventRecurrenceType? RecurrenceType { get; set; }
        public int? Frequency { get; set; }
        public List<EventOccurrenceDto> Occurrences { get; set; } = new();

        public static EventCalendarDto New(Domain.Entities.Event ev, List<EventOccurrenceDto> occurrences)
        {
            return new EventCalendarDto
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Location = ev.Location,
                ChurchId = ev.ChurchId,
                IsRecurring = ev.Recurrence != null && ev.Recurrence.RecurrenceType != Domain.Enum.EventRecurrenceType.None,
                RecurrenceType = ev.Recurrence?.RecurrenceType,
                Frequency = ev.Recurrence?.Frequency,
                Occurrences = occurrences ?? new List<EventOccurrenceDto>()
            };
        }
        public class EventOccurrenceDto
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }
}
