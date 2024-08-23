using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Attributes;
using Discord.Commands;
using Discord.WebSocket;
using GroupAttribute = Discord.Interactions.GroupAttribute;

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

    /// <summary>
    /// Sets the moderator role for the current server.
    /// </summary>
    /// <param name="role">The role to set as moderator.</param>
    [Command("SetModerator", Summary = "Set the moderator role for the current server.")]
    public async Task SetModeratorRoleAsync(SocketRole role)
    {
        await _permissionService.SetModeratorRoleAsync(Context.Guild, role);
        await RespondOrFollowupAsync(text: TranslationService.GetString("moderator-roles.set", TranslationHelper.Arguments("role", role.Name)));
    }

    /// <summary>
    /// Retrieves the current moderator role for the server.
    /// </summary>
    [RequireModeratorRole]
    [Command("GetModeratorRole", Summary = "Get the moderator role for the current server.")]
    public async Task GetModeratorRoleAsync()
    {
        var role = await _permissionService.GetModeratorRoleAsync(Context.Guild);
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
