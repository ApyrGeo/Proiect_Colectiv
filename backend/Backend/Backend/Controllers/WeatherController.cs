using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController(ILogger<WeatherController> logger)
    {
        private readonly ILogger<WeatherController> _logger = logger;

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<object>> Get()
        {
            _logger.LogInformation("Fetching weather forecast...");
            var forecasts = Enumerable.Range(1, 5).Select(index =>
                new
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                }).ToArray();
            _logger.LogInformation("Weather forecast fetched successfully.");
            return await Task.FromResult(forecasts);
        }
    }
}
