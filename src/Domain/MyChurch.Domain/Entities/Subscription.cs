namespace MyChurch.Domain.Entities
{
    public class Subscription
    {
        public Subscription(int planId, DateTime startDate, DateTime endDate)
        {
            PlanId = planId;
            StartDate = startDate;
            EndDate = endDate;
        }

        public int Id { get; set; }
        public int ChurchId { get; set; }
        public Church Church { get; set; }
        public int PlanId { get; set; }
        public Plan Plan { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive => EndDate > DateTime.UtcNow;

        // Pagamentos vinculados à assinatura  
        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
    }
}
