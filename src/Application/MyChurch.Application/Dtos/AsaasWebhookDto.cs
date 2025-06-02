

using System.Text.Json.Serialization;

namespace MyChurch.Application.Dtos
{
    public class AsaasWebhookDto
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("payment")]
        public AsaasWebhookPaymentDto Payment { get; set; }
    }
}
