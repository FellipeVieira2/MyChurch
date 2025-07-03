using System.Text.Json.Serialization;

namespace MyChurch.Domain.Enum
{
    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Cancelled = 3,
        [JsonPropertyName("RECEIVED")]
        Received = 4,
        [JsonPropertyName("CONFIRMED")]
        Confirmed = 5
    }
}