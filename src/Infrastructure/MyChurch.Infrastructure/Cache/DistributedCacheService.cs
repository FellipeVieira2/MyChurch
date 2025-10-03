using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MyChurch.Infrastructure.Cache
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    }

    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public DistributedCacheService(IDistributedCache cache)
        {
            _cache = cache;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            
            if (string.IsNullOrEmpty(cachedData))
                return null;

            return JsonSerializer.Deserialize<T>(cachedData, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
        {
            var serializedData = JsonSerializer.Serialize(value, _jsonOptions);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        {
            // Nota: Esta funcionalidade requer Redis ou implementação customizada
            // Para memória distribuída básica, precisa de solução alternativa
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Extensões para cache de reviews
    /// </summary>
    public static class ReviewCacheExtensions
    {
        public static string GetReviewsCacheKey(int entityId, string entityType, int page, int pageSize)
        {
            return $"reviews:{entityType}:{entityId}:page:{page}:size:{pageSize}";
        }

        public static string GetReviewCacheKey(int reviewId)
        {
            return $"review:{reviewId}";
        }

        public static string GetReviewVotesCacheKey(int reviewId)
        {
            return $"review:{reviewId}:votes";
        }
    }
}
