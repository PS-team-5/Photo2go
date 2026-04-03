namespace Photo2GoAPI.Models;

public class SimilarLocationResult
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string ObjectType { get; init; }
    public required string Category { get; init; }
    public required bool IsUnescoProtected { get; init; }
    public required string ArchitectureStyle { get; init; }
    public required string Period { get; init; }
    public required string City { get; init; }
    public decimal Similarity { get; init; }
    public bool IsOpen { get; init; }
}

