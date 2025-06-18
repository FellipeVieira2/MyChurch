namespace MyChurch.Domain.Entities
{
    public class Donation
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public decimal PlatformFee { get; set; }

        public bool IsTransferred { get; set; } = false;
        public DateTime? TransferredAt { get; set; }

        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
        public ICollection<DonationWorshipService> DonationWorshipServices { get; set; } = new List<DonationWorshipService>();

        // Campanha de arrecadação
        public int? CampaignId { get; private set; }
        public Campaign? Campaign { get; private set; }

        public void SetCampaign(Campaign campaign)
        {
            CampaignId = campaign.Id;
            Campaign = campaign;
        }
    }
}