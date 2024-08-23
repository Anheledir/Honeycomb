using BaseBotService.Core.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PreconditionAttribute = Discord.Commands.PreconditionAttribute;
using PreconditionResult = Discord.Commands.PreconditionResult;

namespace BaseBotService.Core.Attributes;

public class RequireModeratorRoleAttribute : PreconditionAttribute
{
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        IPermissionService permissionService = services.GetRequiredService<IPermissionService>();
        SocketGuildUser? user = context.User as SocketGuildUser;

        return await permissionService.CanUserExecuteModeratorCommandAsync(user)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("You do not have permission to run this command.");
    }


}
