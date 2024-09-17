using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    public class WeatherForecastRequest
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City name must be between 2 and 100 characters.")]
        public required string City { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Country name must be between 2 and 100 characters.")]
        public required string Country { get; set; }
    }
}
