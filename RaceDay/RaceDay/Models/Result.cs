namespace RaceDay.Models;

public class Result
{
    public int Id { get; set; }
    public int RegistrationId { get; set; }
    public int FinishTimeSeconds { get; set; }

    public Registration Registration { get; set; } = null!;
}