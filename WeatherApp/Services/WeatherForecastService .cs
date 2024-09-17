using WeatherApp.Models;
using WeatherApp.Services.ThirdPartyApis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherApp.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IEnumerable<IThirdPartyWeatherApi> _weatherApis;
        private readonly ILogger<WeatherForecastService> _logger;

        public WeatherForecastService(
            IEnumerable<IThirdPartyWeatherApi> weatherApis,
            ILogger<WeatherForecastService> logger)
        {
            _weatherApis = weatherApis;
            _logger = logger;
        }

        public async Task<List<WeatherForecastResult>> GetWeatherForecastAsync(DateTime date, string city, string country)
        {
            _logger.LogInformation("Starting to get weather forecasts for {City}, {Country} on {Date}", city, country, date.ToString("yyyy-MM-dd"));

            var tasks = new List<Task<WeatherForecastResult>>();

            foreach (var api in _weatherApis)
            {
                var apiName = api.GetType().Name;
                _logger.LogInformation("Requesting forecast from {ApiName}", apiName);
                tasks.Add(api.GetForecastAsync(date, city, country));
            }

            var results = await Task.WhenAll(tasks);

            _logger.LogInformation("Successfully retrieved forecasts from all APIs.");

            return new List<WeatherForecastResult>(results);
        }
    }
}
