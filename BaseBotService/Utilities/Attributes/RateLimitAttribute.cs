namespace BaseBotService.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : Attribute
{
    public int MaxAttempts { get; }
    public TimeSpan TimeWindow { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Converting the double into a TimeSpan can't be done with a primary constructor.")]
    public RateLimitAttribute(int maxAttempts, double timeWindowSeconds)
    {
        MaxAttempts = maxAttempts;
        TimeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }
}