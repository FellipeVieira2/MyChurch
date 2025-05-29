namespace MyChurch.Domain.Entities
{
    public class CashFlowCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int ChurchId { get; set; }
        public Church Church { get; set; } = null!;
        public ICollection<CashFlowEntry> CashFlowEntries { get; set; }

    }
}
