using WeatherApp.Models;

namespace WeatherApp.Services
{
    public interface IWeatherForecastService
    {
        Task<List<WeatherForecastResult>> GetWeatherForecastAsync(DateTime date, string city, string country);
    }
}
