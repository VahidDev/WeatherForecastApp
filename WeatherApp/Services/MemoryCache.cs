
using Microsoft.Extensions.Caching.Memory;

namespace WeatherApp.Services
{
    public class MemoryCache : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            _cache.TryGetValue<T>(key, out var value);
            return value;
        }

        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(10)
            });
        }
    }
}
