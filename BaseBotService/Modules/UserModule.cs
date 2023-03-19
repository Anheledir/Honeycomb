using BaseBotService.Enumeration;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BaseBotService.Modules;

[Group("user", "The user management module of Honeycomb.")]
public class UserModule : BaseModule
{
    public IEngagementService EngagementService { get; set; } = null!;

    [UserCommand("User Profile")]
    public async Task UserInfoCommandAsync(IUser user) => await RespondAsync(embed: GetUserProfileEmbed(user, true).Build(), ephemeral: true);

    [SlashCommand("profile", "Returns the profile of the current user, or the user parameter, if one is passed.")]
    public async Task UserinfoCommandAsync(
        [Summary(description: "The users who's profile you want to see, leave empty for yourself.")]
        IUser? user = null
        )
    {
        user ??= Caller;
        await RespondAsync(embed: GetUserProfileEmbed(user, false).Build(), ephemeral: false);
    }

    [SlashCommand("config", "Change the settings of your global Honeycomb profile.")]
    public async Task ConfigProfile()
    {
        var countryMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select the country you're living in.")
            .WithCustomId("usr-profile-country")
            .WithMinValues(0)
            .WithMaxValues(1)
            .AddOptionsFromEnum<Countries>(5, e => e.GetCountryNameWithFlag());

        var languageMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select the languages you can communicate in.")
            .WithCustomId("usr-profile-languages")
            .WithMinValues(0)
            .WithMaxValues(4)
            .AddOptionsFromEnum<Languages>(1, e => e.GetFlaggedLanguageName());

        var timezoneMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select the timezone you're living in.")
            .WithCustomId("usr-profile-timezone")
            .WithMinValues(0)
            .WithMaxValues(1)
            .AddOptionsFromEnum<Timezone>(60, e => e.GetNameWithOffset());

        var genderMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select your gender identity.")
            .WithCustomId("usr-profile-gender")
            .WithMinValues(0)
            .WithMaxValues(1)
            .AddOptionsFromEnum<GenderIdentity>(0, e => e.GetFlaggedGenderName());

        var userConfigMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select the user setting you want to change.")
            .WithCustomId("usr-profile-config")
            .WithMinValues(0)
            .WithMaxValues(1)
            .AddOptionsFromEnum<UserConfigs>(0, e => e.GetUserSettingsName());

        var components = new ComponentBuilder()
            .WithSelectMenu(userConfigMenu)
            .WithButton(new ButtonBuilder("Cancel", "usr-profile-cancel", ButtonStyle.Danger, null, null, isDisabled: true))
            .WithButton(new ButtonBuilder("Save", "usr-profile-save", ButtonStyle.Success, null, null, isDisabled: true));   

        await RespondAsync("The bot sent you a DM with the settings-menu.", ephemeral: true);
        
        IDMChannel dm = await Caller.CreateDMChannelAsync();
        await dm.SendMessageAsync("Please select the setting you want to change.", components: components.Build());
    }

    private EmbedBuilder GetUserProfileEmbed(IUser user, bool includePermissions)
    {
        var result = GetEmbedBuilder()
            .WithTitle(user.Username)
            .WithThumbnailUrl(user.GetAvatarUrl())
            .WithColor(Color.LightOrange);

        var fields = new List<EmbedFieldBuilder>
        {
            new EmbedFieldBuilder
            {
                Name = "Name",
                Value = $"{user} {(user.IsBot ? "🤖" : string.Empty)}{(user.IsWebhook ? "🪝" : string.Empty)}"
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
                        Value = $"{EngagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(DiscordTimestampFormat.ShortDateTime)}\n({EngagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(DiscordTimestampFormat.RelativeTime)})",
                        IsInline = true
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Server points",
                        Value = EngagementService.GetActivityPoints(gUser.GuildId, user.Id).ToString("N0", CultureInfo.InvariantCulture)
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
                                        .Select(p => Regex.Replace(p.ToString(), "([a-z])([A-Z])", "$1 $2"));

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

        result.WithFields(fields);

        return result;
    }
}