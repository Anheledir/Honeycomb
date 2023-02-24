using Discord;
using Discord.Commands;

namespace BaseBotService.Base
{
    internal interface ICommandHandler
    {
        Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result);
        Task InstallCommandsAsync();
    }
}