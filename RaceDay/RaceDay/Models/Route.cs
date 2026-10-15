namespace RaceDay.Models;

public class Route
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public double DistanceKm { get; set; }

    public Category Category { get; set; } = null!;
}