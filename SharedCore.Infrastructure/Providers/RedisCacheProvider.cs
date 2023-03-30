using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using SharedCore.Domain.Abstraction.Providers;

namespace SharedCore.Infrastructure.Providers
{
    public class RedisCacheProvider : IRedisCacheProvider
    {
        private readonly IDistributedCache _cache;

        public RedisCacheProvider(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<T?> GetAsync<T>(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var value = await _cache.GetStringAsync(key);
            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }
        public async Task SetAsync<T>(string? key, T? value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var serializedValue = JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(key, serializedValue);
        }
    }
}
