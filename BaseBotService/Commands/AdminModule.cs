// Ignore Spelling: Admin

using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Infrastructure.Services;
using BaseBotService.Utilities;
using Discord.Commands;
using Discord.WebSocket;

namespace BaseBotService.Commands;

[Group("admin", "Administration of the bot for the current server.")]
[CommandContextType(InteractionContextType.Guild)]
public class AdminModule : BaseModule
{
    public AdminModule(ILogger logger)
    {
        Logger = logger.ForContext<AdminModule>();
    }

    [Command("SetModerator", "Set the moderator role for the current server.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetModeratorRole(SocketRole role)
    {
        PermissionService permissionService = services.GetRequiredService<IPermissionService>();
        permissionService.SetModeratorRole(Context.Guild, role);
        await RespondOrFollowupAsync(text: TranslationService.GetString("moderator-roles.set", TranslationHelper.Arguments("role", role.Name)));
    }

    public async Task GetModeratorRole()
    {
        PermissionService persmissionService = services.GetRequiredService<IPermissionService>();
        SocketRole role = persmissionService.GetModeratorRole(Context.Guild);
        if (role is null)
        {
            await RespondOrFollowupAsync(text: TranslationService.GetString("moderator-roles.none");
        }
        else
        {
            await RespondOrFollowupAsync(text: TranslationService.GetString("moderator-roles", TranslationHelper.Arguments("role", role.Name)));
        }
    }
}
