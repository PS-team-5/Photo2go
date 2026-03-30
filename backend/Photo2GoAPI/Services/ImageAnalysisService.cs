using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Photo2GoAPI.Exceptions;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public class ImageAnalysisService
{
    private const decimal MinimumRecognizedConfidence = 0.7m;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly IImageAnalysisClient _imageAnalysisClient;

    public ImageAnalysisService(IImageAnalysisClient imageAnalysisClient)
    {
        _imageAnalysisClient = imageAnalysisClient;
    }

    public async Task<ImageAnalysisResult> AnalyzeAsync(IFormFile image, CancellationToken cancellationToken = default)
    {
        var aiPayload = await _imageAnalysisClient.AnalyzeAsync(image, cancellationToken);

        try
        {
            // The client returns provider-normalized JSON; this layer validates it against the app contract.
            var analysisResult = JsonSerializer.Deserialize<ImageAnalysisResult>(aiPayload.JsonPayload, JsonOptions);
            if (analysisResult is null)
            {
                throw new AiResponseFormatException("AI atsakymas tuscias.");
            }

            ValidateResult(analysisResult);
            return analysisResult;
        }
        catch (JsonException ex)
        {
            throw new AiResponseFormatException("AI atsakymo nepavyko paversti i strukturuota JSON.", ex);
        }
    }

    private static void ValidateResult(ImageAnalysisResult result)
    {
        if (string.IsNullOrWhiteSpace(result.Name) ||
            string.IsNullOrWhiteSpace(result.ObjectType) ||
            string.IsNullOrWhiteSpace(result.ArchitectureStyle) ||
            string.IsNullOrWhiteSpace(result.Period) ||
            string.IsNullOrWhiteSpace(result.City))
        {
            throw new AiResponseFormatException("AI atsakymas nepilnas.");
        }

        if (result.Confidence < 0 || result.Confidence > 1)
        {
            throw new AiResponseFormatException("AI confidence turi buti tarp 0 ir 1.");
        }

        if (result.Confidence < MinimumRecognizedConfidence)
        {
            throw new ObjectNotRecognizedException(result.Confidence, MinimumRecognizedConfidence);
        }
    }
}
