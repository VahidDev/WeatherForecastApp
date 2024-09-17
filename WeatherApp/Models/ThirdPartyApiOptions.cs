namespace WeatherApp.Models
{
    public class ThirdPartyApiOptions
    {
        public ApiConnectionOptions? OpenWeather { get; set; }
        public ApiConnectionOptions? Weatherbit { get; set; }
        public ApiConnectionOptions? WeatherApi { get; set; }

    }
}
