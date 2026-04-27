using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Photo2GoAPI.Configuration;
using Photo2GoAPI.Exceptions;
using Photo2GoAPI.Models;
using Photo2GoAPI.Services;

namespace Photo2GoAPI.Controllers;

[ApiController]
[Route("")]
public class AnalyzeImageController : ControllerBase
{
    private readonly ImageAnalysisService _imageAnalysisService;
    private readonly ImageUploadService _imageUploadService;
    private readonly SimilarPlaceAlgorithService _similarPlaceAlgorithService;
    private readonly GeneratedRouteService _generatedRouteService;
    private readonly RecommendationFeedbackStore _recommendationFeedbackStore;
    private readonly ImageUploadOptions _options;
    private readonly RouteGenerationOptions _routeGenerationOptions;

    public AnalyzeImageController(
        ImageAnalysisService imageAnalysisService,
        ImageUploadService imageUploadService,
        SimilarPlaceAlgorithService similarPlaceAlgorithService,
        GeneratedRouteService generatedRouteService,
        RecommendationFeedbackStore recommendationFeedbackStore,
        IOptions<ImageUploadOptions> options,
        IOptions<RouteGenerationOptions> routeGenerationOptions)
    {
        _imageAnalysisService = imageAnalysisService;
        _imageUploadService = imageUploadService;
        _similarPlaceAlgorithService = similarPlaceAlgorithService;
        _generatedRouteService = generatedRouteService;
        _recommendationFeedbackStore = recommendationFeedbackStore;
        _options = options.Value;
        _routeGenerationOptions = routeGenerationOptions.Value;
    }

    [HttpPost("analyze-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AnalyzeImage(
        [FromForm] AnalyzeImageRequest request,
        CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(
            TimeSpan.FromSeconds(_routeGenerationOptions.TimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            timeoutCts.Token);
        var timeoutToken = linkedCts.Token;

        try
        {
            var shouldSaveGeneratedRoute = request.UserId is > 0;

            if (shouldSaveGeneratedRoute &&
                !await _generatedRouteService.UserExistsAsync(request.UserId!.Value, timeoutToken))
            {
                return NotFound(new
                {
                    message = "Vartotojas nerastas."
                });
            }

            var validationResult = _imageUploadService.Validate(request.Image);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = validationResult.ErrorMessage,
                    maxFileSizeInBytes = _options.MaxFileSizeInBytes
                });
            }

            var analysisResult = await _imageAnalysisService.AnalyzeAsync(request.Image!, timeoutToken);
            var detectedLocation = await _similarPlaceAlgorithService.FindDetectedLocationAsync(
                analysisResult,
                timeoutToken);
            if (detectedLocation is not null)
            {
                var adjustedConfidence = await _recommendationFeedbackStore.ApplyConfidenceAdjustmentAsync(
                    analysisResult.Confidence,
                    detectedLocation,
                    timeoutToken);

                analysisResult = new ImageAnalysisResult
                {
                    Name = analysisResult.Name,
                    ObjectType = analysisResult.ObjectType,
                    ArchitectureStyle = analysisResult.ArchitectureStyle,
                    Period = analysisResult.Period,
                    City = analysisResult.City,
                    Confidence = adjustedConfidence
                };
            }

            var similarLocations = await _similarPlaceAlgorithService.FindTopSimilarAsync(
                analysisResult,
                cancellationToken: timeoutToken);

            if (shouldSaveGeneratedRoute)
            {
                await _generatedRouteService.SaveAsync(
                    request.UserId!.Value,
                    validationResult.Data!,
                    analysisResult,
                    similarLocations,
                    timeoutToken);
            }

            var routeGenerated = similarLocations.Count > 0;
            var routeMessage = routeGenerated
                ? "Marsrutas sekmingai sugeneruotas."
                : "Marsrutas nesugeneruotas: nerasta panasiu vietu duomenu bazeje.";

            var response = new AnalyzeImageResultResponse
            {
                Message = "Nuotrauka sekmingai priimta analizei.",
                RouteGenerated = routeGenerated,
                RouteMessage = routeMessage,
                File = validationResult.Data!,
                Analysis = analysisResult,
                DetectedLocationId = detectedLocation?.Id,
                DetectedCategory = detectedLocation?.Category,
                SimilarLocations = similarLocations
            };

            return Ok(response);
        }
        catch (OperationCanceledException ex)
            when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new RouteGenerationTimeoutException(
                _routeGenerationOptions.TimeoutSeconds,
                "Marsruto generavimo laikas virsijo SLA riba.",
                ex);
        }
    }

    [HttpPost("analyze-image/feedback")]
    public async Task<IActionResult> SubmitRecommendationFeedback(
        [FromBody] RecommendationFeedbackRequest request,
        CancellationToken cancellationToken)
    {
        if (!SimilarPlaceAlgorithService.IsSupportedFeedback(request.Feedback))
        {
            return BadRequest(new
            {
                message = "Neteisingas atsiliepimo tipas. Naudokite 'Patiko' arba 'Nepatiko'."
            });
        }

        var detectedLocation = await _similarPlaceAlgorithService.FindVilniusLocationByIdAsync(
            request.DetectedLocationId,
            cancellationToken);

        if (detectedLocation is null)
        {
            return NotFound(new
            {
                message = "Atpazinta vieta nerasta Vilniaus duomenu bazeje."
            });
        }

        var normalizedFeedback = request.Feedback.Trim().ToLowerInvariant();
        var similarLocations = await _similarPlaceAlgorithService.FindRecommendationsByFeedbackAsync(
            detectedLocation,
            normalizedFeedback,
            cancellationToken: cancellationToken);

        await _recommendationFeedbackStore.SaveAsync(
            detectedLocation,
            request,
            cancellationToken);

        decimal? adjustedConfidence = request.CurrentConfidence is decimal currentConfidence
            ? await _recommendationFeedbackStore.ApplyConfidenceAdjustmentAsync(
                currentConfidence,
                detectedLocation,
                cancellationToken)
            : null;

        var response = new RecommendationFeedbackResponse
        {
            DetectedLocationId = detectedLocation.Id,
            DetectedCategory = detectedLocation.Category,
            Feedback = normalizedFeedback,
            AdjustedConfidence = adjustedConfidence,
            SimilarLocations = similarLocations
        };

        return Ok(response);
    }
}
