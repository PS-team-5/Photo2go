namespace Photo2GoAPI.Models;

public class AnalyzeImageResultResponse
{
    public required string Message { get; init; }
    public required AnalyzeImageResponse File { get; init; }
    public required ImageAnalysisResult Analysis { get; init; }
    public required IReadOnlyList<SimilarLocationResult> SimilarLocations { get; init; }
}
