namespace Photo2GoAPI.Models;

public class AiImageAnalysisPayload
{
    public required string JsonPayload { get; init; }
    public required string Provider { get; init; }
}
