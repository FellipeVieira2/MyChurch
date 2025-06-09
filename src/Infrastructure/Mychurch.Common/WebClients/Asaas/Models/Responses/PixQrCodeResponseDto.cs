using System.Text.Json.Serialization;

namespace Mychurch.Common.WebClients.Asaas.Models.Responses
{
    public class PixQrCodeResponseDto
    {
        [JsonPropertyName("encodedImage")]
        public string EncodedImage { get; set; }
        [JsonPropertyName("payload")]
        public string Payload { get; set; }
        [JsonPropertyName("expirationDate")]
        public string ExpirationDate { get; set; }
    }
}
