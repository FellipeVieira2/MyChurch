using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MyChurch.Infrastructure.Services
{
    public class GoogleGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleGeocodingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GoogleGeocodingApiKey"];
        }

        public async Task<(double? lat, double? lng)> GetLatLongAsync(string address)
        {
            var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var results = root.GetProperty("results");
            if (results.GetArrayLength() == 0) return (null, null);
            var location = results[0].GetProperty("geometry").GetProperty("location");
            return (location.GetProperty("lat").GetDouble(), location.GetProperty("lng").GetDouble());
        }
    }
}
