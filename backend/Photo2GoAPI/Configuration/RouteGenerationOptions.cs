namespace Photo2GoAPI.Configuration;

public class RouteGenerationOptions
{
    public const string SectionName = "RouteGeneration";

    public int TimeoutSeconds { get; set; } = 15;
}
