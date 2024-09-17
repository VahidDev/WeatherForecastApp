using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;


        public WeatherForecastController(IWeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }


        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] WeatherForecastRequest request)
        {

            var forecasts = await _weatherForecastService.GetWeatherForecastAsync(request.Date, request.City, request.Country);
            return Ok(forecasts);

        }

    }
}
