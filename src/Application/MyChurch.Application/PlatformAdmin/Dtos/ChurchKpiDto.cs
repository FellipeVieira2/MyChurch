namespace MyChurch.Application.PlatformAdmin.Dtos
{
    public class ChurchKpiDto
    {
        public int ChurchId { get; set; }
        public string ChurchName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public int MembersCount { get; set; }
        public int ActiveMembersCount { get; set; }

        public int DonationsCount { get; set; }
        public decimal DonationsTotalAmount { get; set; }
        public decimal DonationsPlatformFeeTotal { get; set; }

        public int TransfersCount { get; set; }
        public decimal TransfersTotalAmount { get; set; }
        public int TransfersPendingCount { get; set; }
        public int TransfersFailedCount { get; set; }

        public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
