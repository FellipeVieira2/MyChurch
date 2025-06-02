using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mychurch.Common.WebClients.Models.Requests
{
    public class AsaasCustomerRequestDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("cpfCnpj")]
        public string CpfCnpj { get; set; }
        [JsonPropertyName("mobilePhone")]
        public string? MobilePhone { get; set; }
        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }
        [JsonPropertyName("addressNumber")]
        public string? AddressNumber { get; set; }
        [JsonPropertyName("externalReference")]
        public string? ExternalReference { get; set; } 
    }
}
