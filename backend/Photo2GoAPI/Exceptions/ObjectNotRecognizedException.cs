namespace Photo2GoAPI.Exceptions;

public class ObjectNotRecognizedException : Exception
{
    public decimal Confidence { get; }
    public decimal MinimumConfidence { get; }

    public ObjectNotRecognizedException(decimal confidence, decimal minimumConfidence)
        : base("Objektas neatpažintas.")
    {
        Confidence = confidence;
        MinimumConfidence = minimumConfidence;
    }
}
