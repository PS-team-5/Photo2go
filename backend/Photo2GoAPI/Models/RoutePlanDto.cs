namespace Photo2GoAPI.Models;

public sealed class RoutePlanDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public required IReadOnlyList<int> LocationIds { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

