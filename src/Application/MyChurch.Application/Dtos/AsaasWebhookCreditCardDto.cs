using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyChurch.Application.Dtos
{
    public class AsaasWebhookCreditCardDto
    {
        [JsonPropertyName("creditCardNumber")]
        public string CreditCardNumber { get; set; }

        [JsonPropertyName("creditCardBrand")]
        public string CreditCardBrand { get; set; }

        [JsonPropertyName("creditCardToken")]
        public string CreditCardToken { get; set; }
    }
}
