using System.Net;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Photo2GoAPI.Configuration;
using Xunit;

namespace Photo2GoAPI.IntegrationTests;


public class AnalyzeImageEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AnalyzeImageEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public void AnalyzeImage_UsesFifteenSecondRouteGenerationTimeout()
    {
        using var scope = _factory.Services.CreateScope();
        var options = scope.ServiceProvider
            .GetRequiredService<IOptions<RouteGenerationOptions>>()
            .Value;

        Assert.Equal(15, options.TimeoutSeconds);
    }

    // Theory leidzia paleisti ta pati testa su keliais skirtingais duomenu rinkiniais.
    [Theory]
    // 1 atvejis: normali nuotrauka turi grazinti 200 OK ir sekmes zinute.
    [InlineData("landmark.jpg", "image/jpeg", HttpStatusCode.OK, "Nuotrauka sekmingai priimta analizei.")]
    // 2 atvejis: jei AI pasitikejimas per mazas, API turi grazinti 400 BadRequest.
    [InlineData("low-confidence.jpg", "image/jpeg", HttpStatusCode.BadRequest, "Objektas neatpazintas")]
    // Vienas testas su dviem testiniais atvejais.
    public async Task AnalyzeImage_ReturnsExpectedResult_ForTwoSimpleCases(
        
        string fileName,
        // Failo MIME tipas, pvz. image/jpeg.
        string mimeType,
       
        HttpStatusCode expectedStatusCode,
      
        string expectedMessageFragment)
    {
        // Sukuriamas multipart/form-data turinys, toks pats kaip siunciant faila is frontendo ar Postman.
        using var content = CreateMultipartContent(fileName, mimeType, new byte[] { 1, 2, 3, 4 });

        
        var response = await _client.PostAsync("/analyze-image", content);
       
        var responseBody = await response.Content.ReadAsStringAsync();

      
        Assert.Equal(expectedStatusCode, response.StatusCode);
       
        Assert.Contains(expectedMessageFragment, responseBody);
    }

    [Fact]
    public async Task AnalyzeImage_ReturnsDetectedVilniusCategoryAndRecommendations()
    {
        using var content = CreateMultipartContent("landmark.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });

        var response = await _client.PostAsync("/analyze-image", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.Equal(2, root.GetProperty("detectedLocationId").GetInt32());
        Assert.Equal("Religious Object", root.GetProperty("detectedCategory").GetString());

        foreach (var location in root.GetProperty("similarLocations").EnumerateArray())
        {
            Assert.Equal("Vilnius", location.GetProperty("city").GetString());
        }
    }

    [Fact]
    public async Task AnalyzeImage_CompletesWithinFifteenSeconds_ForSuccessfulRequests()
    {
        using var content = CreateMultipartContent("landmark.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });
        var stopwatch = Stopwatch.StartNew();

        var response = await _client.PostAsync("/analyze-image", content);
        stopwatch.Stop();

        response.EnsureSuccessStatusCode();
        Assert.True(
            stopwatch.Elapsed <= TimeSpan.FromSeconds(15),
            $"Analyze image request took {stopwatch.Elapsed.TotalSeconds:F2}s.");
    }

    [Fact]
    public async Task AnalyzeImage_ReturnsGatewayTimeout_WhenRouteGenerationExceedsFifteenSeconds()
    {
        using var content = CreateMultipartContent("slow-timeout.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });
        var stopwatch = Stopwatch.StartNew();

        var response = await _client.PostAsync("/analyze-image", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        stopwatch.Stop();

        Assert.Equal(HttpStatusCode.GatewayTimeout, response.StatusCode);
        Assert.Contains("Nepavyko sugeneruoti marsruto per 15 sekundziu", responseBody);
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(17),
            $"Timeout response took {stopwatch.Elapsed.TotalSeconds:F2}s.");
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
}
