using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Photo2GoAPI.Model;

[Table("Routes")]
public class RoutePlan
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    // SQLite doesn't have a native array type, so keep IDs as JSON.
    [Column("location_ids_json")]
    public string LocationIdsJson { get; set; } = "[]";

    [Column("created_at_utc")]
    public DateTime CreatedAtUtc { get; set; }

    public IReadOnlyList<int> GetLocationIds()
        => JsonSerializer.Deserialize<int[]>(LocationIdsJson) ?? Array.Empty<int>();

    public void SetLocationIds(IReadOnlyList<int> locationIds)
        => LocationIdsJson = JsonSerializer.Serialize(locationIds);
}

