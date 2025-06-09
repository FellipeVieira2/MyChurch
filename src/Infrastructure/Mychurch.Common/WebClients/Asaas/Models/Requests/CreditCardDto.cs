
namespace Mychurch.Common.WebClients.Asaas.Models.Requests
{
    public class CreditCardDto
    {
        public string HolderName { get; set; }
        public string Number { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Ccv { get; set; }
    }
}
