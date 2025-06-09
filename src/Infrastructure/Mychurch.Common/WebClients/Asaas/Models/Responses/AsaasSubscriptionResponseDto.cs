using System.Text.Json.Serialization;

namespace Mychurch.Common.WebClients.Asaas.Models.Responses
{
    public class AsaasSubscriptionResponseDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
