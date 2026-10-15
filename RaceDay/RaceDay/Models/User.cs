using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Models;

[Table("User")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("First_Name")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("Last_Name")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Role { get; set; } = "Participant";

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("Password_Hash")]
    public string PasswordHash { get; set; } = string.Empty;

    // Collection initialization simplified using []
    public ICollection<Registration> Registrations { get; set; } = [];
}