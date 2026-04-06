using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace Photo2GoAPI.IntegrationTests;

public class ResponsiveDeviceSupportTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ResponsiveDeviceSupportTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("phone", "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1")]
    [InlineData("tablet", "Mozilla/5.0 (iPad; CPU OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1")]
    public async Task AnalyzeImage_ReturnsSameFunctionalContract_ForPhoneAndTabletClients(
        string deviceType,
        string userAgent)
    {
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var content = CreateMultipartContent("landmark.jpg", "image/jpeg", new byte[] { 1, 2, 3, 4 });

        var response = await client.PostAsync("/analyze-image", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.Equal("Nuotrauka sekmingai priimta analizei.", root.GetProperty("message").GetString());
        Assert.True(root.GetProperty("routeGenerated").GetBoolean());
        Assert.Equal("Marsrutas sekmingai sugeneruotas.", root.GetProperty("routeMessage").GetString());

        var file = root.GetProperty("file");
        Assert.Equal("landmark.jpg", file.GetProperty("originalFileName").GetString());
        Assert.Equal("image/jpeg", file.GetProperty("mimeType").GetString());
        Assert.Equal(4, file.GetProperty("size").GetInt64());

        var analysis = root.GetProperty("analysis");
        Assert.Equal("Vilnius Cathedral", analysis.GetProperty("name").GetString());
        Assert.Equal("Vilnius", analysis.GetProperty("city").GetString());
        Assert.True(analysis.GetProperty("confidence").GetDecimal() >= 0.95m);

        Assert.Equal(2, root.GetProperty("detectedLocationId").GetInt32());
        Assert.Equal("Religious Object", root.GetProperty("detectedCategory").GetString());

        var similarLocations = root.GetProperty("similarLocations").EnumerateArray().ToList();
        Assert.NotEmpty(similarLocations);
        Assert.All(similarLocations, location =>
            Assert.Equal("Vilnius", location.GetProperty("city").GetString()));

        Assert.False(string.IsNullOrWhiteSpace(deviceType));
    }

    private static MultipartFormDataContent CreateMultipartContent(
        string fileName,
        string mimeType,
        byte[] fileContent)
    {
        var multipart = new MultipartFormDataContent();
        var imageContent = new ByteArrayContent(fileContent);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
        multipart.Add(imageContent, "Image", fileName);
        return multipart;
    }
}
