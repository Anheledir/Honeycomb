using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Utilities;

namespace BaseBotService.Commands;

[Group("admin", "Administration of the bot for the current server.")]
[EnabledInDm(false)]
[RequireUserPermission(GuildPermission.Administrator)]
public class AdminModule : BaseModule
{
    private readonly IGuildRepository _guildRepository;
    private readonly ITranslationService _translationService;

    public AdminModule(ILogger logger, ITranslationService TranslationService, IGuildRepository GuildRepository)
    {
        _guildRepository = GuildRepository;
        _translationService = TranslationService;
        Logger = logger.ForContext<AdminModule>();
    }

    [SlashCommand("modrole", "Set the moderator role for the current guild.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetModeratorRoleAsync(
        [Summary("Moderator-Role", "Role that identifies a guild moderator.")]
        IRole modRole
        )
    {
        if (!GuildId.HasValue)
        {
            Logger.Information($"User {Caller.Id} tried to run {nameof(SetModeratorRoleAsync)} from within a DM.");
            await RespondOrFollowupAsync(_translationService.GetString("error-guild-missing"));
            return;
        }

        GuildHC guild = _guildRepository.GetGuild(GuildId.Value, true)!;

        if (guild == null)
        {
            Logger.Error($"Guild ID {GuildId.Value} could not be loaded from the GuildRepository.");
            await RespondOrFollowupAsync(_translationService.GetString("error-guild-load"));
            return;
        }

        // TODO: More than one moderator role should be configurable
        guild.ModeratorRoles.Clear();
        guild.ModeratorRoles.Add(modRole.Id);

        _guildRepository.UpdateGuild(guild);

        await RespondOrFollowupAsync(_translationService.GetAttrString("modrole", "success", TranslationHelper.Arguments("role", modRole.Mention)));
    }
}
