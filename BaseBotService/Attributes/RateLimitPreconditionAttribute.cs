using BaseBotService.Utilities;
using Discord.Commands;
using Serilog;

namespace BaseBotService.Attributes;

public class RateLimitPreconditionAttribute : PreconditionAttribute
{
    public RateLimiter RateLimiter { get; set; } = null!;
    public ILogger Logger { get; set; } = null!;

    private readonly int _maxAttempts;
    private readonly TimeSpan _timeWindow;

    public RateLimitPreconditionAttribute(int maxAttempts, double timeWindowSeconds)
    {
        _maxAttempts = maxAttempts;
        _timeWindow = TimeSpan.FromSeconds(timeWindowSeconds);
    }

    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        ulong userId = context.User.Id;
        string commandName = command.Name;

        Logger.Debug("Checking rate limit for user {UserId} on command {CommandName}", userId, command.Name, _maxAttempts, _timeWindow);


        return await RateLimiter.IsAllowed(userId, commandName, _maxAttempts, _timeWindow)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("You have reached the rate limit for this command. Please wait before trying again.");
    }
}
