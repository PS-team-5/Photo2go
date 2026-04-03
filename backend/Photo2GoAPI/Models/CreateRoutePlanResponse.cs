namespace Photo2GoAPI.Models;

public sealed class CreateRoutePlanResponse
{
    public string Message { get; init; } = string.Empty;
    public required RoutePlanDto Route { get; init; }
}

