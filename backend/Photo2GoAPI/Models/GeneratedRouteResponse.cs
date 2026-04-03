namespace Photo2GoAPI.Models;

public class GeneratedRouteResponse
{
    public required int Id { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required AnalyzeImageResponse File { get; init; }
    public required ImageAnalysisResult Analysis { get; init; }
    public required IReadOnlyList<SimilarLocationResult> SimilarLocations { get; init; }
}
