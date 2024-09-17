using Microsoft.Extensions.Caching.Memory;

namespace WeatherApp.Services
{
    public interface ICacheService
    {

        T? Get<T>(string key);

        void Set<T> (string key, T value);
    }
}
