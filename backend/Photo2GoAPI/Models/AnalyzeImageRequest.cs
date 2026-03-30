using Microsoft.AspNetCore.Http;

namespace Photo2GoAPI.Models;

public class AnalyzeImageRequest
{
    public IFormFile? Image { get; set; }
}
