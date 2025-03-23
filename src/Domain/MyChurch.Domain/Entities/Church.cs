namespace MyChurch.Domain.Entities
{
    public class Church(string name, Address address, string phone, Subscription subscription)
    {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        public Address Address { get; set; } = address;
        public string Phone { get; set; } = phone;
        public ICollection<Member>? Members { get; set; } = new List<Member>();

        // Assinatura da igreja  
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = subscription;
    }
}
