using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services.ThirdPartyApis
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Options;
    using System.Text.Json;
    using StackExchange.Redis;
    using Microsoft.Extensions.Logging;

    namespace WeatherMicroservice.Services.ThirdPartyApis
    {
        public class OpenWeatherApiService : IThirdPartyWeatherApi
        {
            private static readonly string NAME = "OpenWeather";
            private readonly ILogger<OpenWeatherApiService> _logger;
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly ApiConnectionOptions _apiOptions;
            private readonly ICacheService _cacheService;

            public OpenWeatherApiService(
                IHttpClientFactory httpClientFactory,
                IOptions<ThirdPartyApiOptions> options,
                ILogger<OpenWeatherApiService> logger,
                ICacheService cacheService)
            {
                _httpClientFactory = httpClientFactory;
                _apiOptions = options.Value.OpenWeather;
                _logger = logger;
                _cacheService = cacheService;
            }

            public async Task<WeatherForecastResult> GetForecastAsync(DateTime date, string city, string country)
            {
                var cacheKey = $"{_apiOptions.SourceName}:{city}:{country}:{date:yyyy-MM-dd}";
                var cachedResult = _cacheService.Get<WeatherForecastResult>(cacheKey);

                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit for OpenWeather with key: {CacheKey}", cacheKey);
                    return cachedResult;
                }

                _logger.LogInformation("Cache miss for OpenWeather with key: {CacheKey}", cacheKey);

                var result = new WeatherForecastResult { Source = "OpenWeatherMap" };
                var client = _httpClientFactory.CreateClient();

                try
                {
                    var url = $"{_apiOptions.Url}?q={city},{country}&appid={_apiOptions.Key}&units=metric";
                    _logger.LogInformation("Requesting OpenWeatherMap API with URL: {Url}", url);

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug("Received response from OpenWeatherMap API: {Content}", content);

                    var jsonDoc = JsonDocument.Parse(content);
                    var forecastList = jsonDoc.RootElement.GetProperty("list");
                    UnifiedWeatherForecast forecast = null;

                    foreach (var item in forecastList.EnumerateArray())
                    {
                        var forecastDate = DateTime.Parse(item.GetProperty("dt_txt").GetString());
                        if (forecastDate.Date == date.Date)
                        {
                            forecast = new UnifiedWeatherForecast
                            {
                                Source = "OpenWeatherMap",
                                Date = forecastDate,
                                TemperatureCelsius = item.GetProperty("main").GetProperty("temp").GetDouble(),
                                WeatherDescription = item.GetProperty("weather")[0].GetProperty("description").GetString()
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
                        _logger.LogWarning("No forecast available for {Date} from OpenWeatherMap API.", date.Date);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching data from OpenWeatherMap API");
                    result.Error = "An error occurred while fetching data from OpenWeatherMap API.";
                }

                _cacheService.Set(cacheKey, result);

                return result;
            }
        }
    }

}
