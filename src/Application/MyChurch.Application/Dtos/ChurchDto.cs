namespace MyChurch.Application.Dtos
{
    public class ChurchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public AddressDto Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public ICollection<MemberDto> Members { get; set; } = new List<MemberDto>();

        // Assinatura da igreja  
        public SubscriptionDto Subscription { get; set; }
        
        // Banking information (only visible to admin users)
        public BankingInfoDto BankingInfo { get; set; }

        public static ChurchDto New(Domain.Entities.Church church)
        {
            return new ChurchDto
            {
                Id = church.Id,
                Name = church.Name,
                Address = AddressDto.New(church.Address),
                Phone = church.Phone,
                Members = church.Members.Select(MemberDto.New).ToList(),
                Subscription = SubscriptionDto.New(church.Subscription),
                Description = church.Description
            };
        }
    }
}
