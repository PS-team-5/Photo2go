using Photo2GoAPI.Models;
using Photo2GoAPI.Model;
using Photo2GoAPI.Services;
using Xunit;

namespace Photo2GoAPI.IntegrationTests;

public class SimilarityCalculationTests
{
    private static (decimal Score, bool IsSamePlace) InvokeCalculateSimilarity(
        ImageAnalysisResult analysis, Location location)
    {
        var method = typeof(SimilarPlaceAlgorithService)
            .GetMethod("CalculateSimilarity",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static);

        return ((decimal Score, bool IsSamePlace))method!
            .Invoke(null, new object[] { analysis, location })!;
    }

    [Fact]
    public void CalculateSimilarity_WhenAllFieldsMatch_ReturnsSamePlace()
    {
        var analysis = new ImageAnalysisResult
        {
            Name = "Vilnius Cathedral",
            ObjectType = "Cathedral",
            ArchitectureStyle = "Neoclassical",
            Period = "18th century",
            City = "Vilnius",
            Confidence = 0.95m
        };

        var location = new Location
        {
            Name = "Vilnius Cathedral",
            ArchitectureStyle = "Neoclassical",
            Period = "18th century",
            City = "Vilnius"
        };

        var result = InvokeCalculateSimilarity(analysis, location);

        Assert.True(result.IsSamePlace);
        Assert.Equal(1m, result.Score);
    }

    [Fact]
    public void CalculateSimilarity_WhenNoFieldsMatch_ReturnsZeroScore()
    {
        var analysis = new ImageAnalysisResult
        {
            Name = "Eiffel Tower",
            ObjectType = "Tower",
            ArchitectureStyle = "Iron lattice",
            Period = "19th century",
            City = "Paris",
            Confidence = 0.90m
        };

        var location = new Location
        {
            Name = "Vilnius Cathedral",
            ArchitectureStyle = "Neoclassical",
            Period = "18th century",
            City = "Vilnius",
            
        };

        var result = InvokeCalculateSimilarity(analysis, location);

        Assert.False(result.IsSamePlace);
        Assert.Equal(1m/12m, result.Score);
    }
}