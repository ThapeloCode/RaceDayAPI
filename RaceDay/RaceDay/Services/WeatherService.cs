using System.Text.Json;
using RaceDay.DTOs;

namespace RaceDay.Services;

public class WeatherService(HttpClient httpClient) : IWeatherService
{
    public async Task<WeatherForecastDto> GetWeatherForLocationAsync(string location)
    {
        try
        {
            var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(location)}&count=1";
            var geoResponse = await httpClient.GetAsync(geoUrl);

            if (!geoResponse.IsSuccessStatusCode)
                return GetFallbackWeather(location);

            using var geoStream = await geoResponse.Content.ReadAsStreamAsync();
            using var geoDoc = await JsonDocument.ParseAsync(geoStream);

            if (!geoDoc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                return GetFallbackWeather(location);

            var firstMatch = results[0];
            double lat = firstMatch.GetProperty("latitude").GetDouble();
            double lon = firstMatch.GetProperty("longitude").GetDouble();

            var weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";
            var weatherResponse = await httpClient.GetAsync(weatherUrl);

            if (!weatherResponse.IsSuccessStatusCode)
                return GetFallbackWeather(location);

            using var weatherStream = await weatherResponse.Content.ReadAsStreamAsync();
            using var weatherDoc = await JsonDocument.ParseAsync(weatherStream);

            var currentWeather = weatherDoc.RootElement.GetProperty("current_weather");
            double temp = currentWeather.GetProperty("temperature").GetDouble();
            double windSpeed = currentWeather.GetProperty("windspeed").GetDouble();

            return new WeatherForecastDto(location, temp, "Clear Sky", 45, windSpeed);
        }
        catch
        {
            return GetFallbackWeather(location);
        }
    }

    private static WeatherForecastDto GetFallbackWeather(string location) =>
        new(location, 22.0, "Sunny / Mild", 50, 12.5);
}