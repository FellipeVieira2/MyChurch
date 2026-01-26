namespace MyChurch.Application.PlatformAdmin.Dtos
{
    public class PlatformDonationSummaryDto
    {
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }

        public int DonationsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPlatformFee { get; set; }

        public List<PlatformDonationSeriesItemDto> DailySeries { get; set; } = new();
    }

    public class PlatformDonationSeriesItemDto
    {
        public DateTime DateUtc { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFee { get; set; }
    }
}
