using System.Net.Http.Json;
using System.Text.Json;

namespace Mychurch.Common.WebClients.Asaas
{
    public class AsaasWebClient : IAsaasWebClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public AsaasWebClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("access_token", _apiKey);
            _httpClient.BaseAddress ??= new Uri("https://www.asaas.com/api/v3/");
        }

        // CLIENTES
        public async Task<object> CriarClienteAsync(object clienteRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("customers", clienteRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarClienteAsync(string id)
        {
            var response = await _httpClient.GetAsync($"customers/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // COBRANÇAS (BOLETO, PIX, CARTÃO)
        public async Task<object> CriarCobrancaAsync(object cobrancaRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("payments", cobrancaRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
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
