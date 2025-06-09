using System;

namespace MyChurch.Domain.Entities
{
    public class CreditCardInfo
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string Last4Digits { get; set; }
        public string CardBrand { get; set; }
        public string CardHash { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public Member Member { get; set; }
    }
}