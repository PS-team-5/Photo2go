namespace Photo2GoAPI.Models;

public sealed class NotificationMessage
{
    public string Type { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public object? Data { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

