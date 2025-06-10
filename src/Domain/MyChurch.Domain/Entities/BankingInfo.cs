using System;

namespace MyChurch.Domain.Entities
{
    public class BankingInfo
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public string? BankName { get; set; }
        public string? Agency { get; set; }
        public string? Account { get; set; }
        public string? AccountDigit { get; set; }
        public string? AccountType { get; set; }
        public string? HolderName { get; set; }
        public string? HolderDocument { get; set; }
        public string? PixKey { get; set; }
        public string? PixKeyType { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public Church Church { get; set; }
    }
}