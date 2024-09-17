using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services.ThirdPartyApis
{
    public class WeatherApiService : IThirdPartyWeatherApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiConnectionOptions _apiOptions;
        private readonly ILogger<WeatherApiService> _logger;
        private readonly ICacheService _cacheService;

        public WeatherApiService(
            IHttpClientFactory httpClientFactory,
            IOptions<ThirdPartyApiOptions> options,
            ILogger<WeatherApiService> logger,
            ICacheService cacheService)
        {
            _httpClientFactory = httpClientFactory;
            _apiOptions = options.Value.WeatherApi;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<WeatherForecastResult> GetForecastAsync(DateTime date, string city, string country)
        {
            var cacheKey = $"{_apiOptions.SourceName}:{city}:{country}:{date:yyyy-MM-dd}";
            var cachedResult = _cacheService.Get<WeatherForecastResult>(cacheKey);

            if (cachedResult != null)
            {
                _logger.LogInformation("Cache hit for WeatherAPI with key: {CacheKey}", cacheKey);
                return cachedResult;
            }

            _logger.LogInformation("Cache miss for WeatherAPI with key: {CacheKey}", cacheKey);

            var client = _httpClientFactory.CreateClient();
            var result = new WeatherForecastResult { Source = "WeatherAPI" };

            try
            {
                var query = $"{city},{country}";
                var url = $"{_apiOptions.Url}?key={_apiOptions.Key}&q={query}&dt={date:yyyy-MM-dd}&aqi=no&alerts=no";
                _logger.LogInformation("Requesting WeatherAPI with URL: {Url}", url);

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response from WeatherAPI: {Content}", content);

                var jsonDoc = JsonDocument.Parse(content);
                var forecastDay = jsonDoc.RootElement
                    .GetProperty("forecast")
                    .GetProperty("forecastday")[0];

                var forecastDate = DateTime.Parse(forecastDay.GetProperty("date").GetString());
                var day = forecastDay.GetProperty("day");
                var tempCelsius = day.GetProperty("avgtemp_c").GetDouble();
                var condition = day.GetProperty("condition").GetProperty("text").GetString();

                var unifiedForecast = new UnifiedWeatherForecast
                {
                    Source = "WeatherAPI",
                    Date = forecastDate,
                    TemperatureCelsius = tempCelsius,
                    WeatherDescription = condition
                };

                result.Data = unifiedForecast;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data from WeatherAPI");
                result.Error = "An error occurred while fetching data from WeatherAPI.";
            }

            _cacheService.Set(cacheKey, result);

            return result;
        }
    }
}
