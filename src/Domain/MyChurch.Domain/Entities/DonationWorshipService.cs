namespace MyChurch.Domain.Entities
{
    public class DonationWorshipService
    {
        public int DonationId { get; set; }
        public Donation Donation { get; set; }
        public int WorshipServiceId { get; set; }
        public WorshipService WorshipService { get; set; }
    }
}
