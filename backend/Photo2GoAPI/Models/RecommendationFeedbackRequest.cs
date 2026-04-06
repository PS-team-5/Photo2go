namespace Photo2GoAPI.Models;

public class RecommendationFeedbackRequest
{
    public required int DetectedLocationId { get; init; }
    public required string Feedback { get; init; }
    public int? UserId { get; init; }
    public decimal? CurrentConfidence { get; init; }
}
