namespace MyChurch.Domain.Entities
{
    public class Church
    {
        public Church()
        {
        }
        public Church(string name, string phone, Address address, string description)
        {
            Name = name;
            Description = description;
            Phone = phone;
            Address = address;
            Created = DateTime.UtcNow;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string? LogoFileName { get; set; } 
        public string? Description { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public decimal PlatformFee { get; set; } = 0.05m; // valor padrão 5%
        public string Phone { get; set; } 
        public ICollection<Member>? Members { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? AsaasCustomerId { get; set; }
        public string? Document { get; set; }
        public ICollection<Event> Events { get; set; }
        public List<Asset> Assets { get; set; }
        public ICollection<CashFlowCategory> CashFlowCategories { get; set; }
        public ICollection<CashFlowEntry> CashFlowEntries { get; set; }
        public string? OnboardingQrCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Subscription Subscription { get; set; }
        public void Update(string? name, string? phone)
        {
            Name = name ?? Name;
            Phone = phone ?? Phone;
            Updated = DateTime.UtcNow;
        }

        public void UpdateLogo(string logoFileName)
        {
            LogoFileName = logoFileName;
            Updated = DateTime.UtcNow;
        }
    }
}
