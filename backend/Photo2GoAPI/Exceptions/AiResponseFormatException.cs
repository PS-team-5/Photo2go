namespace Photo2GoAPI.Exceptions;

public class AiResponseFormatException : AiIntegrationException
{
    public AiResponseFormatException(string message)
        : base(message)
    {
    }

    public AiResponseFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
