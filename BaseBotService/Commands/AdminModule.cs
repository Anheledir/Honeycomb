using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Utilities;

namespace BaseBotService.Commands;

[Group("admin", "Administration of the bot for the current server.")]
[EnabledInDm(false)]
[RequireUserPermission(GuildPermission.Administrator)]
public class AdminModule(ITranslationService TranslationService, IGuildRepository GuildRepository) : BaseModule
{
    [SlashCommand("modrole", "Set the moderator role for the current guild.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetModeratorRoleAsync(
        [Summary("Moderator-Role", "Role that identifies a guild moderator.")]
        IRole modRole
        )
    {
        if (!GuildId.HasValue)
        {
            await RespondOrFollowupAsync(TranslationService.GetString("error-guild-missing"));
            return;
        }

        GuildHC guild = GuildRepository.GetGuild(GuildId.Value, true)!;

        if (guild == null)
        {
            await RespondOrFollowupAsync(TranslationService.GetString("error-guild-load"));
            return;
        }

        // TODO: More than one moderator role should be configurable
        guild.ModeratorRoles.Clear();
        guild.ModeratorRoles.Add(modRole.Id);

        GuildRepository.UpdateGuild(guild);

        await RespondOrFollowupAsync(TranslationService.GetAttrString("modrole", "success", TranslationHelper.Arguments("role", modRole.Mention)));
    }
}
