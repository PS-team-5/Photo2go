using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Photo2GoAPI.Configuration;
using Photo2GoAPI.Exceptions;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public class OpenAiImageAnalysisClient : IImageAnalysisClient
{
    private const string ResponsesEndpoint = "responses";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly AiOptions _options;
    private readonly ILogger<OpenAiImageAnalysisClient> _logger;

    public OpenAiImageAnalysisClient(
        HttpClient httpClient,
        IOptions<AiOptions> options,
        ILogger<OpenAiImageAnalysisClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AiImageAnalysisPayload> AnalyzeAsync(IFormFile image, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new AiUnavailableException("AI API key nerastas. Patikrink `.env` faila.");
        }

        // The provider accepts image input as a base64 data URL inside the JSON request body.
        var imageBytes = await ReadImageBytesAsync(image, cancellationToken);
        var requestBody = CreateRequestBody(image, imageBytes);
        using var request = new HttpRequestMessage(HttpMethod.Post, ResponsesEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("AI provider returned status {StatusCode}. Body: {Body}", response.StatusCode, rawResponse);
                throw new AiUnavailableException(
                    "AI servisas neatsake sekmingai.",
                    (int)response.StatusCode,
                    rawResponse);
            }

            var jsonPayload = ExtractStructuredPayload(rawResponse);
            return new AiImageAnalysisPayload
            {
                JsonPayload = jsonPayload,
                Provider = _options.Provider
            };
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new AiTimeoutException("Baigesi AI uzklausos laukimo laikas.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new AiUnavailableException("Nepavyko susisiekti su AI servisu.", ex);
        }
    }

    private string CreateRequestBody(IFormFile image, byte[] imageBytes)
    {
        var base64Image = Convert.ToBase64String(imageBytes);
        var mimeType = image.ContentType;
        var dataUrl = $"data:{mimeType};base64,{base64Image}";

        // Ask the model for schema-constrained JSON so the service can deserialize it safely.
        var payload = new
        {
            model = _options.Model,
            input = new object[]
            {
                new
                {
                    role = "system",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_text",
                            text =
                                "Analyze the uploaded image. Return only a valid JSON object with keys: objectType, architectureStyle, period, city, confidence. " +
                                "If a value is unknown, use \"Unknown\". Confidence must be a number from 0 to 1."
                        }
                    }
                },
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_text",
                            text =
                                "Identify the main object or building in the image and infer likely architecture style, historical period, city, and confidence."
                        },
                        new
                        {
                            type = "input_image",
                            image_url = dataUrl
                        }
                    }
                }
            },
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "image_analysis_result",
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        required = new[] { "objectType", "architectureStyle", "period", "city", "confidence" },
                        properties = new
                        {
                            objectType = new { type = "string" },
                            architectureStyle = new { type = "string" },
                            period = new { type = "string" },
                            city = new { type = "string" },
                            confidence = new { type = "number" }
                        }
                    }
                }
            }
        };

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static async Task<byte[]> ReadImageBytesAsync(IFormFile image, CancellationToken cancellationToken)
    {
        await using var stream = image.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    private static string ExtractStructuredPayload(string rawResponse)
    {
        try
        {
            using var document = JsonDocument.Parse(rawResponse);
            var root = document.RootElement;

            // Prefer the flattened output_text field, then fall back to traversing the output array.
            if (root.TryGetProperty("output_text", out var outputText) &&
                outputText.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(outputText.GetString()))
            {
                return outputText.GetString()!;
            }

            if (TryExtractTextFromOutputArray(root, out var extractedText))
            {
                return extractedText;
            }

            throw new AiResponseFormatException("AI atsakyme nerastas strukturuotas tekstas.");
        }
        catch (JsonException ex)
        {
            throw new AiResponseFormatException("Nepavyko nuskaityti AI atsakymo.", ex);
        }
    }

    private static bool TryExtractTextFromOutputArray(JsonElement root, out string jsonPayload)
    {
        jsonPayload = string.Empty;

        if (!root.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var outputItem in output.EnumerateArray())
        {
            if (!outputItem.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty("text", out var textElement) &&
                    textElement.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(textElement.GetString()))
                {
                    jsonPayload = textElement.GetString()!;
                    return true;
                }
            }
        }

        return false;
    }
}
