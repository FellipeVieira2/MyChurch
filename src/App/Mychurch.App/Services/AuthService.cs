using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Mychurch.App.Models;

namespace Mychurch.App.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task<MemberDto?> GetCurrentUserAsync();
        Task LogoutAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string API_BASE_URL = "https://localhost:5210/api"; // ? Porta atualizada para 5210
        private const string TOKEN_KEY = "auth_token";
        private const string USER_KEY = "user_data";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(API_BASE_URL);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/Auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    
                    if (loginResponse != null)
                    {
                        // Salvar token e dados do usuário no armazenamento seguro
                        await SecureStorage.Default.SetAsync(TOKEN_KEY, loginResponse.Token);
                        await SecureStorage.Default.SetAsync(USER_KEY, JsonSerializer.Serialize(loginResponse.Member));
                        
                        return loginResponse;
                    }
                    
                    throw new Exception("Resposta inválida do servidor");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro no login: {error}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Falha na comunicação com o servidor: {ex.Message}");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TOKEN_KEY);
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.Default.GetAsync(TOKEN_KEY);
        }

        public async Task<MemberDto?> GetCurrentUserAsync()
        {
            var userData = await SecureStorage.Default.GetAsync(USER_KEY);
            
            if (!string.IsNullOrEmpty(userData))
            {
                return JsonSerializer.Deserialize<MemberDto>(userData);
            }
            
            return null;
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TOKEN_KEY);
            SecureStorage.Default.Remove(USER_KEY);
            await Task.CompletedTask;
        }
    }
}
