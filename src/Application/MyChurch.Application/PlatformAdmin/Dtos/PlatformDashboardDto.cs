namespace MyChurch.Application.PlatformAdmin.Dtos
{
    public class PlatformDashboardDto
    {
        public int TotalChurches { get; set; }
        public int VerifiedChurches { get; set; }
        public int UnverifiedChurches { get; set; }

        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }

        public decimal DonationsTotalAmount { get; set; }
        public decimal DonationsPlatformFeeTotal { get; set; }
        public int DonationsCount { get; set; }

        public decimal TransfersTotalAmount { get; set; }
        public int TransfersCount { get; set; }
        public int TransfersPendingCount { get; set; }
        public int TransfersFailedCount { get; set; }

        public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
