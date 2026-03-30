using System.Text;
using Microsoft.EntityFrameworkCore;
using Photo2GoAPI.Data;
using Photo2GoAPI.Model;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public class SimilarPlaceAlgorithService
{
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

        var locations = await _db.Locations
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var matches = new List<(Location Location, decimal Score)>(locations.Count);

        foreach (var location in locations)
        {
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
                ArchitectureStyle = match.Location.ArchitectureStyle,
                Period = match.Location.Period,
                City = match.Location.City,
                Similarity = match.Score
            })
            .ToList();
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

