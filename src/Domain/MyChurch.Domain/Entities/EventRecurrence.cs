using System;
using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class EventRecurrence
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public EventRecurrenceType RecurrenceType { get; set; }
        public int Frequency { get; set; }

        /// <summary>
        /// Data/hora final para a recorrência do evento (inclusive).
        /// </summary>
        public DateTime? RecurrenceEndDate { get; set; }
    }
}
