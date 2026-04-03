using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Photo2GoAPI.Configuration;
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
    private readonly ImageUploadOptions _options;

    public AnalyzeImageController(
        ImageAnalysisService imageAnalysisService,
        ImageUploadService imageUploadService,
        SimilarPlaceAlgorithService similarPlaceAlgorithService,
        GeneratedRouteService generatedRouteService,
        IOptions<ImageUploadOptions> options)
    {
        _imageAnalysisService = imageAnalysisService;
        _imageUploadService = imageUploadService;
        _similarPlaceAlgorithService = similarPlaceAlgorithService;
        _generatedRouteService = generatedRouteService;
        _options = options.Value;
    }

    [HttpPost("analyze-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AnalyzeImage(
        [FromForm] AnalyzeImageRequest request,
        CancellationToken cancellationToken)
    {
        var shouldSaveGeneratedRoute = request.UserId is > 0;

        if (shouldSaveGeneratedRoute &&
            !await _generatedRouteService.UserExistsAsync(request.UserId!.Value, cancellationToken))
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

        var analysisResult = await _imageAnalysisService.AnalyzeAsync(request.Image!, cancellationToken);
        var similarLocations = await _similarPlaceAlgorithService.FindTopSimilarAsync(
            analysisResult,
            cancellationToken: cancellationToken);

        if (shouldSaveGeneratedRoute)
        {
            await _generatedRouteService.SaveAsync(
                request.UserId!.Value,
                validationResult.Data!,
                analysisResult,
                similarLocations,
                cancellationToken);
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
            SimilarLocations = similarLocations
        };

        return Ok(response);
    }
}
