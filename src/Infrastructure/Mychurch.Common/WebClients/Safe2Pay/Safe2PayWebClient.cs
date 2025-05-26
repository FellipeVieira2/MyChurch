using System.Net.Http.Json;
using System.Text.Json;

namespace Mychurch.Common.WebClients.Safe2Pay
{
    public class Safe2PayWebClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public Safe2PayWebClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            _httpClient.BaseAddress ??= new Uri("https://api.safe2pay.com.br/");
        }

        // PAGAMENTOS

        public async Task<object> CriarCobrancaAsync(object cobrancaRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("v2/Payment", cobrancaRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarCobrancaAsync(long id)
        {
            var response = await _httpClient.GetAsync($"v2/Payment/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarCobrancaPorReferenciaAsync(string referencia)
        {
            var response = await _httpClient.GetAsync($"v2/Payment?Reference={referencia}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<bool> CancelarCobrancaAsync(long id)
        {
            var response = await _httpClient.PostAsync($"v2/Payment/Cancel/{id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<object> SolicitarReembolsoAsync(object reembolsoRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("v2/Refund", reembolsoRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // CLIENTES

        public async Task<object> CriarClienteAsync(object clienteRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("v2/Customer", clienteRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarClienteAsync(long id)
        {
            var response = await _httpClient.GetAsync($"v2/Customer/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // PLANOS

        public async Task<object> CriarPlanoAsync(object planoRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("v2/Plan", planoRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarPlanoAsync(long id)
        {
            var response = await _httpClient.GetAsync($"v2/Plan/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ListarPlanosAsync()
        {
            var response = await _httpClient.GetAsync("v2/Plan");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        // ASSINATURAS

        public async Task<object> CriarAssinaturaAsync(object assinaturaRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("v2/Subscription", assinaturaRequest, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ConsultarAssinaturaAsync(long id)
        {
            var response = await _httpClient.GetAsync($"v2/Subscription/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> CancelarAssinaturaAsync(long id)
        {
            var response = await _httpClient.PostAsync($"v2/Subscription/Cancel/{id}", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }

        public async Task<object> ListarAssinaturasAsync()
        {
            var response = await _httpClient.GetAsync("v2/Subscription");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<object>(_jsonOptions);
        }
    }
}
