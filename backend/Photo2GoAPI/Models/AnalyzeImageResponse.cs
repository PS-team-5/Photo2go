namespace Photo2GoAPI.Models;

public class AnalyzeImageResponse
{
    public required string OriginalFileName { get; init; }
    public required string MimeType { get; init; }
    public long Size { get; init; }
}
