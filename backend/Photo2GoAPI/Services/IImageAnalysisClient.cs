using Microsoft.AspNetCore.Http;
using Photo2GoAPI.Models;

namespace Photo2GoAPI.Services;

public interface IImageAnalysisClient
{
    Task<AiImageAnalysisPayload> AnalyzeAsync(IFormFile image, CancellationToken cancellationToken = default);
}
