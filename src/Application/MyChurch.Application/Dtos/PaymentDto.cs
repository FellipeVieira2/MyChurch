
using MyChurch.Domain.Entities;

namespace MyChurch.Application.Dtos
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int? SubscriptionId { get; set; }
        public SubscriptionDto Subscription { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed  
        public string TransactionId { get; set; }
        public static PaymentDto New(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Date = payment.Date,
                PaymentStatus = payment.PaymentStatus,
                TransactionId = payment.TransactionId
            };
        }
    }
}
