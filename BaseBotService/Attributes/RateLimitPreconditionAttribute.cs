using BaseBotService.Utilities;
using Discord.Commands;

namespace BaseBotService.Attributes;

public class RateLimitPreconditionAttribute : PreconditionAttribute
{
    private static readonly RateLimiter _rateLimiter = new();
    private readonly int _maxAttempts;
    private readonly TimeSpan _timeWindow;

    public RateLimitPreconditionAttribute(int maxAttempts, double timeWindowSeconds)
    {
        _maxAttempts = maxAttempts;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }

    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        var userId = context.User.Id;
        var commandName = command.Name;

        return await _rateLimiter.IsAllowed(userId, commandName, _maxAttempts, _timeWindow)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("You have reached the rate limit for this command. Please wait before trying again.");
    }
}
