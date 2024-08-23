// Ignore Spelling: Admin

using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using Discord.Commands;
using Discord.WebSocket;
using GroupAttribute = Discord.Interactions.GroupAttribute;
using RequireUserPermissionAttribute = Discord.Commands.RequireUserPermissionAttribute;

namespace BaseBotService.Commands;

[Group("admin", "Administration of the bot for the current server.")]
[CommandContextType(InteractionContextType.Guild)]
public class AdminModule : BaseModule
{
    private readonly ILogger _logger;
    private readonly IPermissionService _permissionService;

    public AdminModule(ILogger logger, IPermissionService permissionService)
    {
        _logger = logger.ForContext<AdminModule>();
        _permissionService = permissionService;
    }

    [Command("SetModerator", Summary = "Set the moderator role for the current server.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetModeratorRole(SocketRole role)
    {
        _permissionService.SetModeratorRole(Context.Guild, role);
        await RespondOrFollowupAsync(text: TranslationService.GetString("moderator-roles.set", TranslationHelper.Arguments("role", role.Name)));
    }

    public async Task GetModeratorRole()
    {
        SocketRole role = _permissionService.GetModeratorRole(Context.Guild);
        if (role is null)
        {
            await RespondOrFollowupAsync(text: TranslationService.GetString("moderator-roles.none"));
        }
        else
        {
            await RespondOrFollowupAsync(text: TranslationService.GetString("moderator-roles", TranslationHelper.Arguments("role", role.Name)));
        }
    }
}
