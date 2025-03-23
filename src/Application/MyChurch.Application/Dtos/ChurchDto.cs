
namespace MyChurch.Application.Dtos
{
    public class ChurchDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AddressDto Address { get; set; }
        public string Phone { get; set; }
        public ICollection<MemberDto> Members { get; set; } = new List<MemberDto>();

        // Assinatura da igreja  
        public int SubscriptionId { get; set; }
        public SubscriptionDto Subscription { get; set; }
    }
}
