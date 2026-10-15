namespace RaceDay.Models;

public class Registration
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public string BibNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Result? Result { get; set; }
}