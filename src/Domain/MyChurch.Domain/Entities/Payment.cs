namespace MyChurch.Domain.Entities
{
    public class Payment
    {
        public Payment(int subscriptionId, decimal amount, DateTime date, string paymentStatus, string transactionId)
        {
            SubscriptionId = subscriptionId;
            Amount = amount;
            Date = date;
            PaymentStatus = paymentStatus;
            TransactionId = transactionId;
        }

        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed  
        public string TransactionId { get; set; }
    }
}
