using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Mychurch.Common.WebClients.Models.Requests;
using Mychurch.Common.WebClients.Models.Responses;

namespace Mychurch.Common.WebClients.Asaas
{
    public class AsaasWebClient : IAsaasWebClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly IConfiguration _config;
        public AsaasWebClient(HttpClient httpClient, IConfiguration config)
        {
            _config = config;

            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("access_token", _config["Assas_Api_Key"]);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "mychurch");
            _httpClient.BaseAddress ??= new Uri(_config["Assas_Base_Url"]);
        }

        // CLIENTES
        public async Task<AsaasCustomerResponseDto> CriarClienteAsync(AsaasCustomerRequestDto clienteRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("customers", clienteRequest, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao criar cliente Asaas: {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<AsaasCustomerResponseDto>(_jsonOptions);
        }

        public async Task<object> ConsultarClienteAsync(string id)
        {
            var response = await _httpClient.GetAsync($"customers/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // COBRANÇAS (BOLETO, PIX, CARTÃO)
        public async Task<AsaasPaymentResponseDto> CriarCobrancaAsync(object cobrancaRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("payments", cobrancaRequest, _jsonOptions);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao criar cliente Asaas: {errorContent}");
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AsaasPaymentResponseDto>(_jsonOptions);
        }

        public async Task<object> ConsultarCobrancaAsync(string id)
        {
            var response = await _httpClient.GetAsync($"payments/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> CancelarCobrancaAsync(string id)
        {
            var response = await _httpClient.PostAsync($"payments/{id}/cancel", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // PIX: Geração de QRCode
        public async Task<object> GerarPixQrCodeAsync(string paymentId)
        {
            var response = await _httpClient.GetAsync($"payments/{paymentId}/pixQrCode");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // ASSINATURAS (RECORRÊNCIA)
        public async Task<object> CriarAssinaturaAsync(object assinaturaRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("subscriptions", assinaturaRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarAssinaturaAsync(string id)
        {
            var response = await _httpClient.GetAsync($"subscriptions/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> CancelarAssinaturaAsync(string id)
        {
            var response = await _httpClient.PostAsync($"subscriptions/{id}/cancel", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // REPASSE (TRANSFERÊNCIAS TIPO MARKETPLACE)
        public async Task<object> CriarTransferenciaAsync(object transferenciaRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("transfers", transferenciaRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarTransferenciaAsync(string id)
        {
            var response = await _httpClient.GetAsync($"transfers/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }
    }
}
