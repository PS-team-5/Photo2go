namespace Photo2GoAPI.Models;

public class ImageAnalysisResult
{
    public required string Name { get; init; }
    public required string ObjectType { get; init; }
    public required string ArchitectureStyle { get; init; }
    public required string Period { get; init; }
    public required string City { get; init; }
    public decimal Confidence { get; init; }
}
