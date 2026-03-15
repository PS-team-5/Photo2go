namespace Photo2GoAPI.Exceptions;

public class AiUnavailableException : AiIntegrationException
{
    public int? StatusCode { get; }
    public string? ProviderResponse { get; }

    public AiUnavailableException(string message)
        : base(message)
    {
    }

    public AiUnavailableException(string message, int? statusCode, string? providerResponse = null)
        : base(message)
    {
        StatusCode = statusCode;
        ProviderResponse = providerResponse;
    }

    public AiUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
