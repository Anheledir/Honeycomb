namespace BaseBotService.Utilities.Attributes;

/// <summary>
/// An attribute that defines rate-limiting constraints for a method.
/// Specifies the maximum number of attempts allowed within a specified time window.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : Attribute
{
    /// <summary>
    /// Gets the maximum number of attempts allowed within the time window.
    /// </summary>
    public int MaxAttempts { get; }

    /// <summary>
    /// Gets the time window for the rate limit.
    /// </summary>
    public TimeSpan TimeWindow { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimitAttribute"/> class.
    /// </summary>
    /// <param name="maxAttempts">The maximum number of attempts allowed.</param>
    /// <param name="timeWindowSeconds">The time window for the rate limit, in seconds.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="maxAttempts"/> is less than or equal to zero,
    /// or if <paramref name="timeWindowSeconds"/> is less than or equal to zero.
    /// </exception>
    public RateLimitAttribute(int maxAttempts, double timeWindowSeconds)
    {
        if (maxAttempts <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxAttempts), "MaxAttempts must be greater than zero.");
        }

        if (timeWindowSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeWindowSeconds), "TimeWindowSeconds must be greater than zero.");
        }

        MaxAttempts = maxAttempts;
        TimeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }
}
