using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Data;
using Photo2GoAPI.Model;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public class RecommendationFeedbackStore
{
    private readonly AppDbContext _db;

    public RecommendationFeedbackStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task SaveAsync(
        Location detectedLocation,
        RecommendationFeedbackRequest request,
        CancellationToken cancellationToken = default)
    {
        var feedback = new StoredRecommendationFeedback
        {
            DetectedLocationId = detectedLocation.Id,
            DetectedCategory = detectedLocation.Category,
            DetectedObjectType = detectedLocation.ObjectType,
            Feedback = request.Feedback.Trim().ToLowerInvariant(),
            UserId = request.UserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.RecommendationFeedback.Add(feedback);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<decimal> ApplyConfidenceAdjustmentAsync(
        decimal baseConfidence,
        Location detectedLocation,
        CancellationToken cancellationToken = default)
    {
        var relevantFeedback = await _db.RecommendationFeedback
            .AsNoTracking()
            .Where(feedback =>
                feedback.DetectedLocationId == detectedLocation.Id ||
                feedback.DetectedCategory == detectedLocation.Category ||
                feedback.DetectedObjectType == detectedLocation.ObjectType)
            .ToListAsync(cancellationToken);

        if (relevantFeedback.Count == 0)
        {
            return baseConfidence;
        }

        var exactLocationSignal = ComputeSignal(
            relevantFeedback.Where(feedback => feedback.DetectedLocationId == detectedLocation.Id));
        var categorySignal = ComputeSignal(
            relevantFeedback.Where(feedback => feedback.DetectedCategory == detectedLocation.Category));
        var objectTypeSignal = ComputeSignal(
            relevantFeedback.Where(feedback => feedback.DetectedObjectType == detectedLocation.ObjectType));

        var adjustment =
            (exactLocationSignal * 0.03m) +
            (categorySignal * 0.05m) +
            (objectTypeSignal * 0.04m);

        return Math.Clamp(baseConfidence + adjustment, 0m, 1m);
    }

    private static decimal ComputeSignal(IEnumerable<StoredRecommendationFeedback> feedback)
    {
        var likes = 0;
        var dislikes = 0;

        foreach (var item in feedback)
        {
            if (item.Feedback == "patiko")
            {
                likes++;
                continue;
            }

            if (item.Feedback == "nepatiko")
            {
                dislikes++;
            }
        }

        var total = likes + dislikes;
        if (total == 0)
        {
            return 0m;
        }

        return (likes - dislikes) / (decimal)(total + 3);
    }
}
