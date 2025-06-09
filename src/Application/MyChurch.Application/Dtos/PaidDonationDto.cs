namespace MyChurch.Application.Dtos
{
    public class PaidDonationDto
    {
        public int DonationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }
    }
}
