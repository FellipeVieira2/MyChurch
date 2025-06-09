using System.Text.Json.Serialization;

namespace Mychurch.Common.WebClients.Asaas.Models.Requests
{
    public class AsaasChargeRequestDto
    {
        [JsonPropertyName("customer")]
        public string AsaasCustomerId { get; set; }
        [JsonPropertyName("value")]
        public decimal Value { get; set; }
        [JsonPropertyName("billingType")]
        public string BillingType { get; set; }
        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("extenalReference")]
        public string ExternalReference { get; set; }
        [JsonPropertyName("creditCard")]
        public CreditCardDto? CreditCard { get; set; }

        [JsonPropertyName("creditCardHolderInfo")]
        public CreditCardHolderInfoDto? CreditCardHolderInfo { get; set; }
        [JsonPropertyName("creditCardToken")]
        public string CreditCardToken { get; set; }
    }
}
