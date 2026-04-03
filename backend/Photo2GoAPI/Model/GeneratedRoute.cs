using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Photo2GoAPI.Model;

[Table("GeneratedRoutes")]
public class GeneratedRoute
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("created_at_utc")]
    public DateTime CreatedAtUtc { get; set; }

    [Required]
    [Column("file_json")]
    public string FileJson { get; set; } = string.Empty;

    [Required]
    [Column("analysis_json")]
    public string AnalysisJson { get; set; } = string.Empty;

    [Required]
    [Column("similar_locations_json")]
    public string SimilarLocationsJson { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
