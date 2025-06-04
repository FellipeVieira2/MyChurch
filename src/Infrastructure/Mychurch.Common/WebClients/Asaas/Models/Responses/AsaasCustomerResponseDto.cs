using System.Text.Json.Serialization;

namespace Mychurch.Common.WebClients.Asaas.Models.Responses
{
    public class AsaasCustomerResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
