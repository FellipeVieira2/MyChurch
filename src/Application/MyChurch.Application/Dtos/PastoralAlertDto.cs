using System;

namespace MyChurch.Application.Dtos
{
    public class PastoralAlertDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
