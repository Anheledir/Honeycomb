using Discord.Commands;

namespace BaseBotService.Base
{
    public class CommandResult : RuntimeResult
    {
        public CommandResult(CommandError? error, string reason) : base(error, reason)
        {
        }
        public static CommandResult FromError(string reason) => new CommandResult(CommandError.Unsuccessful, reason);
        public static CommandResult FromSuccess(string? reason = null) => new CommandResult(null, reason!);
    }
}
