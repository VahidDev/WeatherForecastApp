namespace WeatherApp.Models
{
    public class UnifiedWeatherForecast
    {
        public string? Source { get; set; }
        public DateTime Date { get; set; }
        public double TemperatureCelsius { get; set; }
        public string? WeatherDescription { get; set; }
    }
}
