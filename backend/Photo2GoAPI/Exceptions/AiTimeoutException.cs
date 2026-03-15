namespace Photo2GoAPI.Exceptions;

public class AiTimeoutException : AiIntegrationException
{
    public AiTimeoutException(string message)
        : base(message)
    {
    }

    public AiTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
