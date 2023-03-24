namespace BaseBotService.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : Attribute
{
    public int MaxAttempts { get; }
    public TimeSpan TimeWindow { get; }

    public RateLimitAttribute(int maxAttempts, double timeWindowSeconds)
    {
        MaxAttempts = maxAttempts;
        TimeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }
}
