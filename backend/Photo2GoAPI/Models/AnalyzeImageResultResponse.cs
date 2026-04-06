namespace Photo2GoAPI.Models;

public class AnalyzeImageResultResponse
{
    public required string Message { get; init; }

    // Route generation status based on the AI analysis + similar locations selection.
    public required bool RouteGenerated { get; init; }
    public required string RouteMessage { get; init; }

    public required AnalyzeImageResponse File { get; init; }
    public required ImageAnalysisResult Analysis { get; init; }
    public int? DetectedLocationId { get; init; }
    public string? DetectedCategory { get; init; }
    public required IReadOnlyList<SimilarLocationResult> SimilarLocations { get; init; }
}
