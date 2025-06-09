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

        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
    }
}