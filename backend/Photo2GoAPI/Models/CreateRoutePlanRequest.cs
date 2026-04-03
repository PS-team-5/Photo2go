namespace Photo2GoAPI.Models;

public sealed class CreateRoutePlanRequest
{
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<int> LocationIds { get; init; } = Array.Empty<int>();
}

