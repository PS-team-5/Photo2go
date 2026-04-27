namespace Photo2GoAPI.Exceptions;

public class RouteGenerationTimeoutException : Exception
{
    public int TimeoutSeconds { get; }

    public RouteGenerationTimeoutException(int timeoutSeconds, string message)
        : base(message)
    {
        TimeoutSeconds = timeoutSeconds;
    }

    public RouteGenerationTimeoutException(int timeoutSeconds, string message, Exception innerException)
        : base(message, innerException)
    {
        TimeoutSeconds = timeoutSeconds;
    }
}
