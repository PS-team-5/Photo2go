using System.Text;
using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Data;
using Photo2GoAPI.Model;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public class SimilarPlaceAlgorithService
{
    private const string SupportedCity = "Vilnius";
    private readonly AppDbContext _db;

    public SimilarPlaceAlgorithService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SimilarLocationResult>> FindTopSimilarAsync(
        ImageAnalysisResult analysis,
        int take = 3,
        CancellationToken cancellationToken = default)
    {
        if (take <= 0)
        {
            return Array.Empty<SimilarLocationResult>();
        }

        var locations = await GetVilniusLocationsAsync(cancellationToken);
        var detectedLocation = ResolveDetectedLocation(analysis, locations);

        var matches = new List<(Location Location, decimal Score)>(locations.Count);

        foreach (var location in locations)
        {
            if (detectedLocation is not null && location.Id == detectedLocation.Id)
            {
                continue;
            }

            var (score, isSamePlace) = CalculateSimilarity(analysis, location);
            if (isSamePlace)
            {
                continue;
            }

            matches.Add((location, score));
        }

        return matches
            .OrderByDescending(match => match.Score)
            .ThenBy(match => match.Location.Name, StringComparer.OrdinalIgnoreCase)
            .Take(take)
            .Select(match => new SimilarLocationResult
            {
                Id = match.Location.Id,
                Name = match.Location.Name,
                ObjectType = match.Location.ObjectType,
                Category = match.Location.Category,
                IsUnescoProtected = IsUnescoProtected(match.Location),
                ArchitectureStyle = match.Location.ArchitectureStyle,
                Period = match.Location.Period,
                City = match.Location.City,
                Similarity = match.Score,
                IsOpen = IsLocationOpen(match.Location)
            })
            .ToList();
    }

    public async Task<Location?> FindDetectedLocationAsync(
        ImageAnalysisResult analysis,
        CancellationToken cancellationToken = default)
    {
        var locations = await GetVilniusLocationsAsync(cancellationToken);
        return ResolveDetectedLocation(analysis, locations);
    }

    public async Task<Location?> FindVilniusLocationByIdAsync(
        int locationId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Locations
            .AsNoTracking()
            .Where(location => location.Id == locationId)
            .Where(location => location.City == SupportedCity)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SimilarLocationResult>> FindRecommendationsByFeedbackAsync(
        Location detectedLocation,
        string feedback,
        int take = 3,
        CancellationToken cancellationToken = default)
    {
        if (take <= 0)
        {
            return Array.Empty<SimilarLocationResult>();
        }

        var locations = await GetVilniusLocationsAsync(cancellationToken);
        var normalizedFeedback = NormalizeFeedback(feedback);
        var matches = new List<(Location Location, decimal Score)>(locations.Count);

        foreach (var location in locations)
        {
            if (location.Id == detectedLocation.Id)
            {
                continue;
            }

            var hasSameCategory = string.Equals(
                location.Category,
                detectedLocation.Category,
                StringComparison.OrdinalIgnoreCase);

            if (normalizedFeedback == "patiko" && !hasSameCategory)
            {
                continue;
            }

            if (normalizedFeedback == "nepatiko" && hasSameCategory)
            {
                continue;
            }

            var (score, _) = CalculateSimilarity(
                new ImageAnalysisResult
                {
                    Name = detectedLocation.Name,
                    ObjectType = detectedLocation.ObjectType,
                    ArchitectureStyle = detectedLocation.ArchitectureStyle,
                    Period = detectedLocation.Period,
                    City = detectedLocation.City,
                    Confidence = 1m
                },
                location);

            matches.Add((location, score));
        }

        return matches
            .OrderByDescending(match => match.Score)
            .ThenBy(match => match.Location.Name, StringComparer.OrdinalIgnoreCase)
            .Take(take)
            .Select(match => new SimilarLocationResult
            {
                Id = match.Location.Id,
                Name = match.Location.Name,
                ObjectType = match.Location.ObjectType,
                Category = match.Location.Category,
                IsUnescoProtected = IsUnescoProtected(match.Location),
                ArchitectureStyle = match.Location.ArchitectureStyle,
                Period = match.Location.Period,
                City = match.Location.City,
                Similarity = match.Score,
                IsOpen = IsLocationOpen(match.Location)
            })
            .ToList();
    }

    public static bool IsSupportedFeedback(string feedback)
    {
        var normalizedFeedback = NormalizeFeedback(feedback);
        return normalizedFeedback is "patiko" or "nepatiko";
    }

    private async Task<List<Location>> GetVilniusLocationsAsync(CancellationToken cancellationToken)
    {
        return await _db.Locations
            .AsNoTracking()
            .Where(location => location.City == SupportedCity)
            .ToListAsync(cancellationToken);
    }

    private static Location? ResolveDetectedLocation(
        ImageAnalysisResult analysis,
        IReadOnlyList<Location> locations)
    {
        if (locations.Count == 0)
        {
            return null;
        }

        return locations
            .Select(location =>
            {
                var (score, _) = CalculateSimilarity(analysis, location);
                return new { Location = location, Score = score };
            })
            .OrderByDescending(match => match.Score)
            .ThenBy(match => match.Location.Name, StringComparer.OrdinalIgnoreCase)
            .Select(match => match.Location)
            .FirstOrDefault();
    }

    private static string NormalizeFeedback(string feedback)
    {
        return string.IsNullOrWhiteSpace(feedback)
            ? string.Empty
            : feedback.Trim().ToLowerInvariant();
    }

    private static bool IsLocationOpen(Location location)
    {
        if (location.OpeningTime is null || location.ClosingTime is null)
        {
            return false;
        }

        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
        var openingTime = location.OpeningTime.Value;
        var closingTime = location.ClosingTime.Value;

        if (openingTime <= closingTime)
        {
            return currentTime >= openingTime && currentTime < closingTime;
        }

        return currentTime >= openingTime || currentTime < closingTime;
    }

    private static bool IsUnescoProtected(Location location)
    {
        if (string.IsNullOrWhiteSpace(location.IsUnescoProtected))
        {
            return false;
        }

        var value = location.IsUnescoProtected.Trim();
        return value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("y", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("taip", StringComparison.OrdinalIgnoreCase);
    }

    private static (decimal Score, bool IsSamePlace) CalculateSimilarity(ImageAnalysisResult analysis, Location location)
    {
        var nameScore = ComputeFieldSimilarity(analysis.Name, location.Name);
        var styleScore = ComputeFieldSimilarity(analysis.ArchitectureStyle, location.ArchitectureStyle);
        var periodScore = ComputeFieldSimilarity(analysis.Period, location.Period);
        var cityScore = ComputeFieldSimilarity(analysis.City, location.City);

        // Equal weights for name/style/period/city.
        var totalScore = (nameScore + styleScore + periodScore + cityScore) / 4m;

        // "Same place" is considered a 100% match on the currently checked fields.
        var isSamePlace =
            nameScore == 1m &&
            styleScore == 1m &&
            periodScore == 1m &&
            cityScore == 1m;

        return (totalScore, isSamePlace);
    }

    private static decimal ComputeFieldSimilarity(string? left, string? right)
    {
        var normalizedLeft = NormalizeValue(left);
        var normalizedRight = NormalizeValue(right);

        if (string.IsNullOrWhiteSpace(normalizedLeft) || string.IsNullOrWhiteSpace(normalizedRight))
        {
            return 0m;
        }

        if (IsUnknown(normalizedLeft) || IsUnknown(normalizedRight))
        {
            return 0m;
        }

        if (string.Equals(normalizedLeft, normalizedRight, StringComparison.Ordinal))
        {
            return 1m;
        }

        // Token Jaccard similarity (handles "Baroque/Renaissance" vs "Renaissance", etc).
        var leftTokens = Tokenize(normalizedLeft);
        var rightTokens = Tokenize(normalizedRight);

        if (leftTokens.Count == 0 || rightTokens.Count == 0)
        {
            return 0m;
        }

        var intersectionCount = leftTokens.Intersect(rightTokens).Count();
        var unionCount = leftTokens.Union(rightTokens).Count();

        if (unionCount == 0)
        {
            return 0m;
        }

        return (decimal)intersectionCount / unionCount;
    }

    private static string NormalizeValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToLowerInvariant();
    }

    private static bool IsUnknown(string value)
    {
        return value is "unknown" or "n/a" or "na" or "null" or "?";
    }

    private static HashSet<string> Tokenize(string value)
    {
        var tokens = new HashSet<string>(StringComparer.Ordinal);
        var buffer = new StringBuilder(value.Length);

        foreach (var ch in value)
        {
            if (char.IsLetterOrDigit(ch))
            {
                buffer.Append(ch);
                continue;
            }

            FlushToken(tokens, buffer);
        }

        FlushToken(tokens, buffer);
        return tokens;
    }

    private static void FlushToken(HashSet<string> tokens, StringBuilder buffer)
    {
        if (buffer.Length == 0)
        {
            return;
        }

        var token = buffer.ToString();
        buffer.Clear();

        if (token.Length <= 1)
        {
            return;
        }

        // Minimal stopword filtering to improve name/style matching.
        if (token is "the" or "of" or "and" or "st" or "saint")
        {
            return;
        }

        tokens.Add(token);
    }
}

