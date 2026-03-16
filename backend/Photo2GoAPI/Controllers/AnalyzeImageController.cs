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
    private readonly ImageUploadOptions _options;

    public AnalyzeImageController(
        ImageAnalysisService imageAnalysisService,
        ImageUploadService imageUploadService,
        SimilarPlaceAlgorithService similarPlaceAlgorithService,
        IOptions<ImageUploadOptions> options)
    {
        _imageAnalysisService = imageAnalysisService;
        _imageUploadService = imageUploadService;
        _similarPlaceAlgorithService = similarPlaceAlgorithService;
        _options = options.Value;
    }

    [HttpPost("analyze-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AnalyzeImage(
        [FromForm] AnalyzeImageRequest request,
        CancellationToken cancellationToken)
    {
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

        var response = new AnalyzeImageResultResponse
        {
            Message = "Nuotrauka sekmingai priimta analizei.",
            File = validationResult.Data!,
            Analysis = analysisResult,
            SimilarLocations = similarLocations
        };

        return Ok(response);
    }
}
