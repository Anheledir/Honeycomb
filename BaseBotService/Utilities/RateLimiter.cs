namespace BaseBotService.Utilities;

using System;
using System.Collections.Concurrent;

public class RateLimiter
{
    private readonly ConcurrentDictionary<(ulong, string), (DateTime, int)> _userCommandUsage;

    public RateLimiter()
    {
        _userCommandUsage = new ConcurrentDictionary<(ulong, string), (DateTime, int)>();
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<bool> IsAllowed(ulong userId, string command, int maxAttempts, TimeSpan timeWindow)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        (DateTime timestamp, int count) = _userCommandUsage.GetOrAdd((userId, command), (DateTime.UtcNow, 0));

        if (DateTime.UtcNow - timestamp > timeWindow)
        {
            _userCommandUsage[(userId, command)] = (DateTime.UtcNow, 1);
            return true;
        }

        if (count >= maxAttempts)
        {
            return false;
        }

        _userCommandUsage[(userId, command)] = (timestamp, count + 1);
        return true;
    }
}