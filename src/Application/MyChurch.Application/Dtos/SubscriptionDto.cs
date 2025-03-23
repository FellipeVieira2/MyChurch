
namespace MyChurch.Application.Dtos
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public int ChurchId { get; set; }
        public ChurchDto Church { get; set; }
        public int PlanId { get; set; }
        public PlanDto Plan { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive => EndDate > DateTime.UtcNow;

        // Pagamentos vinculados à assinatura  
        public ICollection<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }
}
