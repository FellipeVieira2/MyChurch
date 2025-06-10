using System;

namespace MyChurch.Domain.Entities
{
    public class TransferHistory
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? Status { get; set; } // Ex: Pending, Completed, Failed
        public string? Notes { get; set; }
        public Church Church { get; set; }
    }
}