using BaseBotService.Commands.Enums;
using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Utilities.Extensions;
using Discord.WebSocket;

namespace BaseBotService.Commands;

[Group("admin", "Administration of the bot for the current server.")]
[EnabledInDm(false)]
[RequireContext(ContextType.Guild)]
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

    [SlashCommand("guild", "Change the settings for the current guild.")]
    public async Task ConfigGuildAsync()
    {
        await RespondOrFollowupAsync(_translationService.GetString("guild-config"), components: ShowGuildConfigMenu().Build(), ephemeral: true);
    }

    private ComponentBuilder ShowGuildConfigMenu()
    {
        var guildConfigMenu = new SelectMenuBuilder()
            .WithPlaceholder(_translationService.GetString("guild-config"))
            .WithCustomId("admin.guild.config")
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOptionsFromEnum<GuildConfigs>(-1, e => e.GetGuildSettingsName(_translationService));

        return new ComponentBuilder()
            .WithSelectMenu(guildConfigMenu)
            .WithButton(new ButtonBuilder(_translationService.GetString("button-close"), "guild.config.close", ButtonStyle.Primary));
    }

    [ComponentInteraction("guild.config.close", ignoreGroupNames: true)]
    public async Task CloseGuildConfigAsync()
    {
        await DeferAsync();
        SocketMessageComponent component = (SocketMessageComponent)Context.Interaction;
        await component.ModifyOriginalResponseAsync(x =>
        {
            x.Content = _translationService.GetString("guild-config-saved");
            x.Components = null;
        });
        await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => component.DeleteOriginalResponseAsync());
    }

    [ComponentInteraction("guild.config.main", ignoreGroupNames: true)]
    public async Task GoBackProfileMainAsync()
    {
        await DeferAsync();
        SocketMessageComponent component = (SocketMessageComponent)Context.Interaction;
        await component.ModifyOriginalResponseAsync(x =>
        {
            x.Content = _translationService.GetString("guild-config");
            x.Components = ShowGuildConfigMenu().Build();
        });
    }

    [ComponentInteraction("admin.guild.config", ignoreGroupNames: true)]
    public async Task AdminGuildConfigAsync(string[] selections)
    {
        GuildConfigs selection = Enum.Parse<GuildConfigs>(selections.FirstOrDefault() ?? "0");

        SelectMenuBuilder configSetting = new();
        string message = string.Empty;

        if (!GuildId.HasValue)
        {
            Logger.Information($"User {Caller.Id} tried to run {nameof(AdminGuildConfigAsync)} from within a DM.");
            await RespondOrFollowupAsync(_translationService.GetString("error-guild-missing"), ephemeral: true);
            return;
        }

        GuildHC guild = _guildRepository.GetGuild(GuildId.Value, true)!;

        if (guild == null)
        {
            Logger.Error($"Guild ID {GuildId.Value} could not be loaded from the GuildRepository.");
            await RespondOrFollowupAsync(_translationService.GetString("error-guild-load"), ephemeral: true);
            return;
        }

        switch (selection)
        {
            case GuildConfigs.Modroles:
                configSetting.WithPlaceholder(_translationService.GetAttrString("guild-config", "modrole"))
                    .WithCustomId("guild.config.save.modrole")
                    .WithMinValues(1)
                    .WithMaxValues(5);
                _ = await configSetting.GetSelectMenuBuilderAsync(Context.Client, GuildId.Value, guild.ModeratorRoles, Logger);
                message = _translationService.GetAttrString("guild-config", "modrole");
                break;
            default:
                Logger.Error($"User {Context.User.Id} selected unhandled enum {selection}.");
                break;
        }

        await DeferAsync();

        ComponentBuilder components = new ComponentBuilder()
            .WithSelectMenu(configSetting)
            .WithButton(new ButtonBuilder(_translationService.GetString("button-back"), "guild.config.main", ButtonStyle.Primary));

        await ModifyOriginalResponseAsync(x =>
        {
            x.Content = message;
            x.Components = components.Build();
        });
    }

    [ComponentInteraction("guild.config.save.modrole", ignoreGroupNames: true)]
    public async Task SaveGuildModroleAsync(string[] selections)
    {
        GuildHC guild = _guildRepository.GetGuild(GuildId!.Value, true)!;

        if (guild == null)
        {
            Logger.Error($"Guild ID {GuildId.Value} could not be loaded from the GuildRepository.");
            await RespondOrFollowupAsync(_translationService.GetString("error-guild-load"), ephemeral: true);
            return;
        }

        guild.ModeratorRoles.Clear();
        guild.ModeratorRoles.AddRange(selections.Select(x => ulong.Parse(x)));
        _guildRepository.UpdateGuild(guild);

        await DeferAsync();
    }
}
