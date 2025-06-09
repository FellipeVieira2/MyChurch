

using System.Text.Json.Serialization;

namespace MyChurch.Application.Dtos
{
    public class AsaasWebhookEventDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("dateCreated")]
        public string DateCreated { get; set; }

        [JsonPropertyName("payment")]
        public AsaasWebhookPaymentDto Payment { get; set; }
    }
}
