using System;
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class EventNotificationDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime SentAt { get; set; }
        public string Message { get; set; }

        public static EventNotificationDto New(EventNotification notification)
        {
            if (notification == null) return null;

            return new EventNotificationDto
            {
                Id = notification.Id,
                EventId = notification.EventId,
                SentAt = notification.SentAt,
                Message = notification.Message
            };
        }
    }
}
