using BaseBotService.Commands.Interfaces;
using BaseBotService.Core.Base;
using BaseBotService.Interactions.Enums;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Enums;
using BaseBotService.Utilities.Extensions;
using Discord.WebSocket;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BaseBotService.Commands;

[Group("user", "The user management module of Honeycomb.")]
public class UserModule : BaseModule
{
    private readonly IEngagementService _engagementService;

    public UserModule(ILogger logger, IEngagementService engagementService)
    {
        Logger = logger.ForContext<UserModule>();
        _engagementService = engagementService;
    }

    [UserCommand("User Profile")]
    public async Task UserInfoCommandAsync(IUser user) => await RespondAsync(
        ephemeral: true,
        embed: GetUserProfileEmbed(user, true).Build());

    [SlashCommand("profile", "Returns the profile of the current user, or the user parameter, if one is passed.")]
    public async Task UserinfoCommandAsync(
        [Summary(description: "The users who's profile you want to see, leave empty for yourself.")]
        IUser? user = null
        )
    {
        user ??= Caller;
        await RespondAsync(ephemeral: false, embed: GetUserProfileEmbed(user, false).Build());
    }

    [SlashCommand("config", "Change the settings of your global Honeycomb profile.")]
    public async Task ConfigProfile()
    {
        SelectMenuBuilder userConfigMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select the user setting you want to change, or click cancel to exit.")
            .WithCustomId("usr-profile-config")
            .WithMinValues(0)
            .WithMaxValues(1)
            .AddOptionsFromEnum<UserConfigs>(0, e => e.GetUserSettingsName());

        ComponentBuilder components = new ComponentBuilder()
            .WithSelectMenu(userConfigMenu)
            .WithButton(new ButtonBuilder("Close", "usr-profile-close", ButtonStyle.Primary));

        await RespondAsync("The bot sent you a DM with the settings-menu.", ephemeral: true);

        IDMChannel dm = await Caller.CreateDMChannelAsync();
        _ = await dm.SendMessageAsync("Please select the setting you want to change.", components: components.Build());
    }

    public Task UserProfileCountry(SocketInteractionContext ctx)
    {
        SocketMessageComponent component = (SocketMessageComponent)ctx.Interaction;
        UserConfigs selection = Enum.Parse<UserConfigs>(component.Data.Values.First());

        SelectMenuBuilder configSetting = new();
        string message = string.Empty;

        switch (selection)
        {
            case UserConfigs.Country:
                configSetting.WithPlaceholder("Select the country you are living in.")
                    .WithCustomId("usr-profile-country")
                    .WithMinValues(0)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Countries>(0, e => e.GetCountryNameWithFlag());
                message = "Please select the country you are living in.";
                break;
            case UserConfigs.Languages:
                configSetting.WithPlaceholder("Select up to four languages you can communicate in.")
                    .WithCustomId("usr-profile-languages")
                    .WithMinValues(0)
                    .WithMaxValues(4)
                    .AddOptionsFromEnum<Languages>(0, e => e.GetFlaggedLanguageName());
                message = "Please select up to four languages you can communicate in.";
                break;
            case UserConfigs.GenderIdentity:
                configSetting.WithPlaceholder("Select your preferred gender identity.")
                    .WithCustomId("usr-profile-gender")
                    .WithMinValues(0)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<GenderIdentity>(0, e => e.GetFlaggedGenderName());
                message = "Please select your preferred gender identity.";
                break;
            case UserConfigs.Timezone:
                configSetting.WithPlaceholder("Select the timezone you are living in.")
                    .WithCustomId("usr-profile-timezone")
                    .WithMinValues(0)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Timezone>(0, e => e.GetNameWithOffset());
                message = "Please select the timezone you are living in.";
                break;
            case UserConfigs.Birthday:
                Logger.Debug($"User {ctx.User.Id} selected 4 {selection}.");
                break;
            default:
                Logger.Error($"User {ctx.User.Id} selected unhandled enum {selection}.");
                break;
        }

        ComponentBuilder components = new ComponentBuilder()
            .WithSelectMenu(configSetting)
            .WithButton(new ButtonBuilder("Go back", "usr-profile-main", ButtonStyle.Primary));

        component.ModifyOriginalResponseAsync(x => x.Content = message);
        component.ModifyOriginalResponseAsync(x => x.Components = components.Build());
        return Task.CompletedTask;
    }

    private EmbedBuilder GetUserProfileEmbed(IUser user, bool includePermissions)
    {
        EmbedBuilder result = GetEmbedBuilder()
            .WithTitle(user.Username)
            .WithThumbnailUrl(user.GetAvatarUrl())
            .WithColor(Color.LightOrange);

        List<EmbedFieldBuilder> fields = new()
        {
            new EmbedFieldBuilder
            {
                Name = "Name",
                Value = $"{user} {(user.IsBot ? UnicodeEmojiHelper.robotFace : string.Empty)}{(user.IsWebhook ? UnicodeEmojiHelper.satelliteAntenna : string.Empty)}"
            },
            new EmbedFieldBuilder
            {
                Name = "Created at",
                Value = $"{user.CreatedAt.ToDiscordTimestamp(DiscordTimestampFormat.ShortDateTime)}\n({user.CreatedAt.ToDiscordTimestamp(DiscordTimestampFormat.RelativeTime)})",
                IsInline = true
            }
        };

        if (user is IGuildUser gUser)
        {
            fields.Add(
            new EmbedFieldBuilder
            {
                Name = "Joined at",
                Value = $"{gUser.JoinedAt?.ToDiscordTimestamp(DiscordTimestampFormat.ShortDateTime)}\n({gUser.JoinedAt?.ToDiscordTimestamp(DiscordTimestampFormat.RelativeTime)})",
                IsInline = true
            });

            if (!user.IsBot && !user.IsWebhook)
            {
                fields.AddRange(new[] {
                    new EmbedFieldBuilder
                    {
                        Name = "Last active",
                        Value = $"{_engagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(DiscordTimestampFormat.ShortDateTime)}\n({_engagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(DiscordTimestampFormat.RelativeTime)})",
                        IsInline = true
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Server points",
                        Value = _engagementService.GetActivityPoints(gUser.GuildId, user.Id).ToString("N0", CultureInfo.InvariantCulture)
                    }
                });
            }

            IEnumerable<string> roleMentions = ((SocketGuildUser)gUser).Roles.Where(x => !x.IsEveryone).Select(x => x.Mention);
            fields.Add(
                new EmbedFieldBuilder
                {
                    Name = "Roles",
                    Value = roleMentions.Any() ? string.Join(", ", roleMentions) : "None"
                });

            if (includePermissions)
            {
                IEnumerable<string> permissionNames = Enum.GetValues(typeof(GuildPermission))
                                        .Cast<GuildPermission>()
                                        .Where(gUser.GuildPermissions.Has)
                                        .Select(p => p.ToString().FromCamelCase());

                fields.Add(
                new EmbedFieldBuilder
                {
                    Name = "Permissions",
                    Value = permissionNames.Any() ? string.Join(", ", permissionNames) : "None"
                });
            }
            result.Title = $"{gUser.DisplayName} @ {gUser.Guild.Name}";
            result.ThumbnailUrl = gUser.GetDisplayAvatarUrl();
        }

        _ = result.WithFields(fields);

        return result;
    }
}