using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models;

[Table("Event")]
public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Column("Start_Date")]
    public DateTime StartDate { get; set; }

    [Column("End_Date")]
    public DateTime EndDate { get; set; }

    [StringLength(150)]
    public string Location { get; set; } = string.Empty;

    // Collection initialization simplified using []
    public ICollection<Category> Categories { get; set; } = [];
}