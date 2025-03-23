namespace MyChurch.Domain.Entities
{
    public class Church(string name, Address address, string phone, Subscription subscription, string? logo)
    {
        public int Id { get; set; }
        public string Name { get; set; } = name;
        public string? Logo { get; set; } = logo;
        public string? Description { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; } = address;
        public string Phone { get; set; } = phone;
        public ICollection<Member>? Members { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        // Assinatura da igreja  
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = subscription;

        public void Update(string? name, string? phone)
        {
            Name = name ?? Name;
            Phone = phone ?? Phone;
            Updated = DateTime.UtcNow;
        }
    }
}
