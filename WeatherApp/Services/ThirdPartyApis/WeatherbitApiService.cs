using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services.ThirdPartyApis
{
    public class WeatherbitApiService : IThirdPartyWeatherApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiConnectionOptions _apiOptions;
        private readonly ILogger<WeatherbitApiService> _logger;
        private readonly ICacheService _cacheService;

        public WeatherbitApiService(
             IHttpClientFactory httpClientFactory,
             IOptions<ThirdPartyApiOptions> options,
             ILogger<WeatherbitApiService> logger,
             ICacheService cacheService)
        {
            _httpClientFactory = httpClientFactory;
            _apiOptions = options.Value.Weatherbit;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<WeatherForecastResult> GetForecastAsync(DateTime date, string city, string country)
        {
            var cacheKey = $"{_apiOptions.SourceName}:{city}:{country}:{date:yyyy-MM-dd}";
            var cachedResult = _cacheService.Get<WeatherForecastResult>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation("Cache hit for Weatherbit with key: {CacheKey}", cacheKey);
                return cachedResult;
            }

            _logger.LogInformation("Cache miss for Weatherbit with key: {CacheKey}", cacheKey);

            var result = new WeatherForecastResult { Source = "Weatherbit" };
            var client = _httpClientFactory.CreateClient();

            try
            {
                var url = $"{_apiOptions.Url}?city={city}&country={country}&key={_apiOptions.Key}&units=M";
                _logger.LogInformation("Requesting Weatherbit API with URL: {Url}", url);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response from Weatherbit API: {Content}", content);

                var jsonDoc = JsonDocument.Parse(content);
                var dataList = jsonDoc.RootElement.GetProperty("data");

                UnifiedWeatherForecast? forecast = null;

                foreach (var item in dataList.EnumerateArray())
                {
                    var datetimeStr = item.GetProperty("datetime").GetString();
                    var forecastDate = DateTime.Parse(datetimeStr);

                    if (forecastDate.Date == date.Date)
                    {
                        forecast = new UnifiedWeatherForecast
                        {
                            Source = "Weatherbit",
                            Date = forecastDate,
                            TemperatureCelsius = item.GetProperty("temp").GetDouble(),
                            WeatherDescription = item.GetProperty("weather").GetProperty("description").GetString()
                        };
                        break;
                    }
                }

                if (forecast != null)
                {
                    result.Data = forecast;
                }
                else
                {
                    result.Error = "No forecast available for the specified date.";
                    _logger.LogWarning("No forecast available for {Date} from Weatherbit API.", date.Date);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from Weatherbit API");
                result.Error = "An error occurred while fetching data from Weatherbit API.";
            }

            _cacheService.Set(cacheKey, result);

            return result;
        }
    }
}
