
namespace MyChurch.Application.Dtos
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public SubscriptionDto Subscription { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed  
        public string TransactionId { get; set; }
    }
}
