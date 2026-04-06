namespace Photo2GoAPI.Models;

public class RecommendationFeedbackResponse
{
    public required int DetectedLocationId { get; init; }
    public required string DetectedCategory { get; init; }
    public required string Feedback { get; init; }
    public decimal? AdjustedConfidence { get; init; }
    public required IReadOnlyList<SimilarLocationResult> SimilarLocations { get; init; }
}
