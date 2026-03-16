using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Photo2GoAPI.Model;
[Table("Locations")]

public class Location
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string ObjectType { get; set; } = null!;
    public string ArchitectureStyle { get; set; } = null!;
    public string Period { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Province { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string BuildingMaterials { get; set; } = null!;
    public string IsUnescoProtected { get; set; }
}