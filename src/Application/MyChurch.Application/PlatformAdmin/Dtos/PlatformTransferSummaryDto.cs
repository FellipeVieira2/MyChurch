namespace MyChurch.Application.PlatformAdmin.Dtos
{
    public class PlatformTransferSummaryDto
    {
        public DateTime StartDateUtc { get; set; }
        public DateTime EndDateUtc { get; set; }

        public int TransfersCount { get; set; }
        public int PendingCount { get; set; }
        public int FailedCount { get; set; }
        public int CompletedCount { get; set; }

        public decimal TotalAmount { get; set; }
        public List<PlatformTransferSeriesItemDto> DailySeries { get; set; } = new();
    }

    public class PlatformTransferSeriesItemDto
    {
        public DateTime DateUtc { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
        public int PendingCount { get; set; }
        public int FailedCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
