using MyChurch.Domain.Enum;

namespace MyChurch.Domain.Entities
{
    public class CashFlowEntry
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public CashFlowType Type { get; set; }
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;
        public int? MemberId { get; set; }
        public Member? Member { get; set; }
        public int CategoryId { get; set; }
        public CashFlowCategory Category { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
    }
}
