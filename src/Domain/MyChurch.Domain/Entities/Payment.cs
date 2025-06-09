namespace MyChurch.Domain.Entities
{
    public class Payment
    {
        public Payment(decimal amount, DateTime date, string paymentStatus, string transactionId, string billingType = null)
        {
            Amount = amount;
            Date = date;
            PaymentStatus = paymentStatus;
            TransactionId = transactionId;
            BillingType = billingType;
        }

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string PaymentStatus { get; set; } // Pending, Completed, Failed  
        public string TransactionId { get; set; }
        public string? BillingType { get; set; } // PIX, BOLETO, CREDIT_CARD, etc.

        // Relacionamento polimórfico
        public int? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public int? DonationId { get; set; }
        public Donation? Donation { get; set; }
    }
}