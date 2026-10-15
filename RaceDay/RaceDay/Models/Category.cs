using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Route = RaceDay.Models.Route;

namespace RaceDay.Models;

[Table("Category")]
public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("Event_Id")]
    public int EventId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    // Navigation Properties
    [ForeignKey(nameof(EventId))]
    public Event Event { get; set; } = null!;

    public Route? Route { get; set; }

    // Collection initialization simplified using []
    public ICollection<Registration> Registrations { get; set; } = [];
}