using Microsoft.AspNetCore.Http;

namespace Photo2GoAPI.Models;

public class AnalyzeImageRequest
{
    public int UserId { get; set; }
    public IFormFile? Image { get; set; }
}
