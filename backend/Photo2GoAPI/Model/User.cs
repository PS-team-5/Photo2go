using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Photo2GoAPI.Model;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
[Table("Users")]
public class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("username")]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password")]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    public ICollection<GeneratedRoute> GeneratedRoutes { get; set; } = new List<GeneratedRoute>();
}
