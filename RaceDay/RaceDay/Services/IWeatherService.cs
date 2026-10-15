using RaceDay.DTOs;

namespace RaceDay.Services;

public interface IWeatherService
{
    Task<WeatherForecastDto> GetWeatherForLocationAsync(string location);
}