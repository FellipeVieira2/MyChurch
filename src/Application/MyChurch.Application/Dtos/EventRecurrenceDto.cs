using System;
using MyChurch.Domain.Entities;
using MyChurch.Domain.Enum;

namespace MyChurch.Application.Dtos
{
    public class EventRecurrenceDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public EventRecurrenceType RecurrenceType { get; set; }
        public int Frequency { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }

        public static EventRecurrenceDto New(EventRecurrence recurrence)
        {
            if (recurrence == null) return null;

            return new EventRecurrenceDto
            {
                Id = recurrence.Id,
                EventId = recurrence.EventId,
                RecurrenceType = recurrence.RecurrenceType,
                Frequency = recurrence.Frequency,
                RecurrenceEndDate = recurrence.RecurrenceEndDate
            };
        }
    }
}
