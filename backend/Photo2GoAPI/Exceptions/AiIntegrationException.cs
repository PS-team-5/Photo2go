namespace Photo2GoAPI.Exceptions;

public class AiIntegrationException : Exception
{
    public AiIntegrationException(string message)
        : base(message)
    {
    }

    public AiIntegrationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
