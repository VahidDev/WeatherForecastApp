using WeatherApp.Models;

namespace WeatherApp.Services.ThirdPartyApis
{
    public interface IThirdPartyWeatherApi
    {
        Task<WeatherForecastResult> GetForecastAsync(DateTime date, string city, string country);
    }
}
