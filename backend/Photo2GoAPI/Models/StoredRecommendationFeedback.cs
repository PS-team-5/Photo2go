using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Photo2GoAPI.Model;

namespace Photo2GoAPI.Models;

[Table("RecommendationFeedback")]
public class StoredRecommendationFeedback
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public int DetectedLocationId { get; set; }
    public Location DetectedLocation { get; set; } = null!;

    [MaxLength(150)]
    public string DetectedCategory { get; set; } = string.Empty;

    [MaxLength(100)]
    public string DetectedObjectType { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Feedback { get; set; } = string.Empty;

    public int? UserId { get; set; }
    public User? User { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
