using Microsoft.AspNetCore.Mvc;
using RaceDay.Services;

namespace RaceDay.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherController(IWeatherService weatherService) : ControllerBase
{
    [HttpGet("{location}")]
    public async Task<IActionResult> GetRaceDayWeather(string location)
    {
        var forecast = await weatherService.GetWeatherForLocationAsync(location);
        return Ok(forecast);
    }
}