using System;

namespace MyChurch.Domain.Entities
{
    public class EventNotification
    {
        public int Id { get; set; } // id_notificacao (PK)
        public int EventId { get; set; } // id_evento (FK)
        public Event Event { get; set; }
        public DateTime SentAt { get; set; } // data_envio
        public string Message { get; set; } // mensagem
    }
}