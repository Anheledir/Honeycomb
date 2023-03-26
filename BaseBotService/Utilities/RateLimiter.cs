using System.Collections.Concurrent;

namespace BaseBotService.Utilities;
public class RateLimiter
{
    private readonly ConcurrentDictionary<(ulong, string), (DateTime, int)> _userCommandUsage;
    private readonly ILogger _logger;

    public RateLimiter(ILogger logger)
    {
        _userCommandUsage = new ConcurrentDictionary<(ulong, string), (DateTime, int)>();
        _logger = logger.ForContext<RateLimiter>();
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<bool> IsAllowed(ulong userId, string command, int maxAttempts, TimeSpan timeWindow)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        (DateTime timestamp, int count) = _userCommandUsage.GetOrAdd((userId, command), (DateTime.UtcNow, 0));

        if (DateTime.UtcNow - timestamp > timeWindow)
        {
            _logger.Debug("Resetting rate limit for user {UserId} on command {CommandName}", userId, command);
            _userCommandUsage[(userId, command)] = (DateTime.UtcNow, 1);
            return true;
        }

        if (count >= maxAttempts)
        {
            _logger.Debug("Rate limit exceeded for user {UserId} on command {CommandName}", userId, command);
            return false;
        }

        _logger.Debug("Incrementing rate limit for user {UserId} on command {CommandName}", userId, command);
        _userCommandUsage[(userId, command)] = (timestamp, count + 1);
        return true;
    }
}