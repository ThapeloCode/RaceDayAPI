namespace RaceDay.DTOs;

// Authentication DTOs
public record RegisterDto(string FirstName, string LastName, string Email, string Password, string? Role);
public record LoginDto(string Email, string Password);
public record UserResponseDto(int Id, string FirstName, string LastName, string Email, string Role);

// Event DTOs
public record CreateEventDto(string Title, string Description, DateTime StartDate, DateTime EndDate, string Location);
public record EventResponseDto(int Id, string Title, string Description, DateTime StartDate, DateTime EndDate, string Location);

// Category DTOs
public record CreateCategoryDto(string Title);
public record CategoryResponseDto(int Id, int EventId, string Title);

// Route DTOs
public record CreateRouteDto(string Title, string Location, double DistanceKm);
public record RouteResponseDto(int Id, int CategoryId, string Title, string Location, double DistanceKm);

// Registration DTOs
public record CreateRegistrationDto(int UserId, int CategoryId, string BibNumber);
public record RegistrationResponseDto(int Id, int UserId, int CategoryId, string BibNumber, DateTime Date);

// Result DTOs
public record RecordResultDto(int FinishTimeSeconds);
public record ResultResponseDto(int Id, int RegistrationId, int FinishTimeSeconds, string ParticipantName, string BibNumber);

// Live Weather Integration DTO
public record WeatherForecastDto(string Location, double TemperatureCelsius, string Condition, int Humidity, double WindSpeedKmh);