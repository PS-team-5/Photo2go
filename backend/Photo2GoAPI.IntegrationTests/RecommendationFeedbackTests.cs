using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Photo2GoAPI.Data;
using Xunit;

namespace Photo2GoAPI.IntegrationTests;

public class RecommendationFeedbackTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public RecommendationFeedbackTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        ResetDatabase();
    }

    [Fact]
    public async Task SubmitFeedback_WithLike_PersistsFeedback_AndReturnsVilniusDatabaseRecommendations()
    {
        var initialAnalysis = await AnalyzeAsync("landmark.jpg");
        var detectedLocationId = initialAnalysis.GetProperty("detectedLocationId").GetInt32();
        var currentConfidence = initialAnalysis.GetProperty("analysis").GetProperty("confidence").GetDecimal();

        using var response = await _client.PostAsync(
            "/analyze-image/feedback",
            CreateJsonContent($$"""
                                {
                                  "detectedLocationId": {{detectedLocationId}},
                                  "feedback": "Patiko",
                                  "currentConfidence": {{currentConfidence}}
                                }
                                """));

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;
        var recommendations = root.GetProperty("similarLocations").EnumerateArray().ToArray();

        Assert.NotEmpty(recommendations);
        Assert.Equal("patiko", root.GetProperty("feedback").GetString());
        Assert.True(root.GetProperty("adjustedConfidence").GetDecimal() > currentConfidence);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbLocationIds = db.Locations.Select(location => location.Id).ToHashSet();
        var savedFeedback = db.RecommendationFeedback.Single();

        Assert.Equal(detectedLocationId, savedFeedback.DetectedLocationId);
        Assert.Equal("patiko", savedFeedback.Feedback);

        foreach (var location in recommendations)
        {
            var id = location.GetProperty("id").GetInt32();
            Assert.Contains(id, dbLocationIds);
            Assert.Equal("Vilnius", location.GetProperty("city").GetString());
            Assert.Equal("Religious Object", location.GetProperty("category").GetString());
        }
    }

    [Fact]
    public async Task SubmitFeedback_WithDislike_ReturnsDifferentCategoryRecommendations_OnlyFromVilnius()
    {
        var initialAnalysis = await AnalyzeAsync("landmark.jpg");
        var currentConfidence = initialAnalysis.GetProperty("analysis").GetProperty("confidence").GetDecimal();

        using var response = await _client.PostAsync(
            "/analyze-image/feedback",
            CreateJsonContent($$"""
                                {
                                  "detectedLocationId": 2,
                                  "feedback": "Nepatiko",
                                  "currentConfidence": {{currentConfidence}}
                                }
                                """));

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(responseBody);
        var recommendations = document.RootElement
            .GetProperty("similarLocations")
            .EnumerateArray()
            .ToArray();

        Assert.NotEmpty(recommendations);

        foreach (var location in recommendations)
        {
            Assert.Equal("Vilnius", location.GetProperty("city").GetString());
            Assert.NotEqual("Religious Object", location.GetProperty("category").GetString());
        }
    }

    [Fact]
    public async Task SubmitFeedback_WithLike_IncreasesConfidence_ForLaterSearch()
    {
        var initialAnalysis = await AnalyzeAsync("landmark.jpg");
        var initialConfidence = initialAnalysis.GetProperty("analysis").GetProperty("confidence").GetDecimal();

        using var feedbackResponse = await _client.PostAsync(
            "/analyze-image/feedback",
            CreateJsonContent($$"""
                                {
                                  "detectedLocationId": 2,
                                  "feedback": "Patiko",
                                  "currentConfidence": {{initialConfidence}}
                                }
                                """));

        feedbackResponse.EnsureSuccessStatusCode();

        var nextAnalysis = await AnalyzeAsync("landmark.jpg");
        var nextConfidence = nextAnalysis.GetProperty("analysis").GetProperty("confidence").GetDecimal();

        Assert.True(nextConfidence > initialConfidence);
    }

    [Fact]
    public async Task SubmitFeedback_WithDislike_DecreasesConfidence_ForLaterSearch()
    {
        var initialAnalysis = await AnalyzeAsync("landmark.jpg");
        var initialConfidence = initialAnalysis.GetProperty("analysis").GetProperty("confidence").GetDecimal();

        using var feedbackResponse = await _client.PostAsync(
            "/analyze-image/feedback",
            CreateJsonContent($$"""
                                {
                                  "detectedLocationId": 2,
                                  "feedback": "Nepatiko",
                                  "currentConfidence": {{initialConfidence}}
                                }
                                """));

        feedbackResponse.EnsureSuccessStatusCode();

        var nextAnalysis = await AnalyzeAsync("landmark.jpg");
        var nextConfidence = nextAnalysis.GetProperty("analysis").GetProperty("confidence").GetDecimal();

        Assert.True(nextConfidence < initialConfidence);
    }

    [Fact]
    public async Task SubmitFeedback_WithInvalidFeedback_ReturnsBadRequest()
    {
        using var response = await _client.PostAsync(
            "/analyze-image/feedback",
            CreateJsonContent("""
                              {
                                "detectedLocationId": 2,
                                "feedback": "Maybe"
                              }
                              """));

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Neteisingas atsiliepimo tipas", responseBody);
    }

    [Fact]
    public async Task SubmitFeedback_ForUnknownLocation_ReturnsNotFound()
    {
        using var response = await _client.PostAsync(
            "/analyze-image/feedback",
            CreateJsonContent("""
                              {
                                "detectedLocationId": 9999,
                                "feedback": "Patiko"
                              }
                              """));

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Atpazinta vieta nerasta", responseBody);
    }

    private async Task<JsonElement> AnalyzeAsync(string fileName)
    {
        using var content = CreateMultipartContent(fileName, "image/jpeg", new byte[] { 1, 2, 3, 4 });
        using var response = await _client.PostAsync("/analyze-image", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(responseBody);
        return document.RootElement.Clone();
    }

    private void ResetDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    private static MultipartFormDataContent CreateMultipartContent(
        string fileName,
        string mimeType,
        byte[] fileContent)
    {
        var multipart = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(fileContent);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
        multipart.Add(imageContent, "Image", fileName);
        return multipart;
    }

    private static StringContent CreateJsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
