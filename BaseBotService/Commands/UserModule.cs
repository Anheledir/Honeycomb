using BaseBotService.Commands.Enums;
using BaseBotService.Commands.Interfaces;
using BaseBotService.Commands.Modals;
using BaseBotService.Core.Base;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Enums;
using BaseBotService.Utilities.Extensions;
using Discord.Interactions;
using Discord.WebSocket;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BaseBotService.Commands;

[Group("user", "The user management module of Honeycomb.")]
public class UserModule : BaseModule
{
    private readonly IEngagementService _engagementService;
    private readonly IMemberHCRepository _memberRepository;

    public UserModule(ILogger logger, IEngagementService engagementService, IMemberHCRepository memberRepository)
    {
        Logger = logger.ForContext<UserModule>();
        _engagementService = engagementService;
        _memberRepository = memberRepository;
    }

    [UserCommand("User Profile")]
    public async Task UserInfoCommandAsync(IUser user) =>
        await RespondOrFollowupAsync(
            embed: GetUserProfileEmbed(user, true).Build(),
            ephemeral: true);

    [SlashCommand("profile", "Returns the profile of the current user, or the user parameter, if one is passed.")]
    public async Task UserinfoCommandAsync(
        [Summary(description: "The users who's profile you want to see, leave empty for yourself.")]
        IUser? user = null
        )
    {
        user ??= Caller;
        await RespondOrFollowupAsync(embed: GetUserProfileEmbed(user, false).Build(), ephemeral: false);
    }

    [SlashCommand("config", "Change the settings of your global Honeycomb profile.")]
    public async Task ConfigProfileAsync()
    {
        await RespondOrFollowupInDMAsync("Please select the setting you want to change.", components: ShowUserConfigMenu().Build());
    }

    private static ComponentBuilder ShowUserConfigMenu()
    {
        var userConfigMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select the user setting you want to change, or click cancel to exit.")
            .WithCustomId("user.profile.config")
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOptionsFromEnum<UserConfigs>(-1, e => e.GetUserSettingsName());

        return new ComponentBuilder()
            .WithSelectMenu(userConfigMenu)
            .WithButton(new ButtonBuilder("Close", "user.profile.close", ButtonStyle.Primary));
    }

    [ComponentInteraction("user.profile.close", ignoreGroupNames: true)]
    public async Task CloseUserProfileAsync()
    {
        await DeferAsync();
        SocketMessageComponent component = (SocketMessageComponent)Context.Interaction;
        await component.ModifyOriginalResponseAsync(x =>
        {
            x.Content = "All settings saved.";
            x.Components = null;
        });
        await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ => component.DeleteOriginalResponseAsync());
    }

    [ComponentInteraction("user.profile.main", ignoreGroupNames: true)]
    public async Task GoBackProfileMainAsync()
    {
        await DeferAsync();
        SocketMessageComponent component = (SocketMessageComponent)Context.Interaction;
        await component.ModifyOriginalResponseAsync(x =>
        {
            x.Content = "Please select the setting you want to change.";
            x.Components = ShowUserConfigMenu().Build();
        });
    }

    [ComponentInteraction("user.profile.save.country", ignoreGroupNames: true)]
    public async Task SaveProfileCountryAsync(string[] selections)
    {
        Countries selection = Enum.Parse<Countries>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true);

        member.Country = selection;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.save.gender", ignoreGroupNames: true)]
    public async Task SaveProfileGenderIdentityAsync(string[] selections)
    {
        GenderIdentity selection = Enum.Parse<GenderIdentity>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true);

        member.GenderIdentity = selection;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.save.timezone", ignoreGroupNames: true)]
    public async Task SaveProfileTimezoneAsync(string[] selections)
    {
        Timezone selection = Enum.Parse<Timezone>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true);

        member.Timezone = selection;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ModalInteraction("user.profile.save.birthday", ignoreGroupNames: true)]
    public async Task SaveProfileBirthday(UserProfileSaveBirthdayModal data)
    {
        if (data?.Validate() == true)
        {
            MemberHC member = _memberRepository.GetUser(Caller.Id, true);
            member.Birthday = data.GetBirthday();
            _memberRepository.UpdateUser(member);
        }
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.save.languages", ignoreGroupNames: true)]
    public async Task SaveProfileLanguagesAsync(string[] selections)
    {
        Languages selectedLanguages = 0;

        foreach (var value in selections)
        {
            if (Enum.TryParse<Languages>(value, out var language))
            {
                selectedLanguages |= language;
            }
        }

        MemberHC member = _memberRepository.GetUser(Caller.Id, true);

        member.Languages = selectedLanguages;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.config", ignoreGroupNames: true)]
    public async Task UserProfileConfigAsync(string[] selections)
    {
        UserConfigs selection = Enum.Parse<UserConfigs>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true);

        SelectMenuBuilder configSetting = new();
        string message = string.Empty;

        switch (selection)
        {
            case UserConfigs.Country:
                configSetting.WithPlaceholder("Select the country you are living in.")
                    .WithCustomId("user.profile.save.country")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Countries>((int)member.Country, e => e.GetCountryNameWithFlag());
                message = "Please select the country you are living in.";
                break;
            case UserConfigs.Languages:
                configSetting.WithPlaceholder("Select up to four languages you can communicate in.")
                    .WithCustomId("user.profile.save.languages")
                    .WithMinValues(1)
                    .WithMaxValues(4)
                    .AddOptionsFromEnum<Languages>((int)member.Languages, e => e.GetFlaggedLanguageName());
                message = "Please select up to four languages you can communicate in.";
                break;
            case UserConfigs.GenderIdentity:
                configSetting.WithPlaceholder("Select your preferred gender identity.")
                    .WithCustomId("user.profile.save.gender")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<GenderIdentity>((int)member.GenderIdentity, e => e.GetFlaggedGenderName());
                message = "Please select your preferred gender identity.";
                break;
            case UserConfigs.Timezone:
                configSetting.WithPlaceholder("Select the timezone you are living in.")
                    .WithCustomId("user.profile.save.timezone")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Timezone>((int)member.Timezone, e => e.GetNameWithOffset());
                message = "Please select the timezone you are living in.";
                break;
            case UserConfigs.Birthday:
                var mb = new ModalBuilder()
                    .WithTitle("Birthday")
                    .WithCustomId("user.profile.save.birthday")
                    .AddTextInput("Day", "day", placeholder: "29", maxLength: 2, value: member.Birthday?.Day.ToString("D2"))
                    .AddTextInput("Month", "month", placeholder: "03", maxLength: 2, value: member.Birthday?.Month.ToString("D2"))
                    .AddTextInput(
                        "Year",
                        "year",
                        placeholder: $"{DateTime.UtcNow.Year - 18} or leave empty",
                        maxLength: 4,
                        required: false,
                        value:
                            ((member.Birthday?.Year) ?? 0) < DateTime.UtcNow.Year - 100
                                ? string.Empty
                                : member.Birthday?.Year.ToString("D2"));
                await RespondWithModalAsync(mb.Build());
                return;
            default:
                Logger.Error($"User {Context.User.Id} selected unhandled enum {selection}.");
                break;
        }

        await DeferAsync();

        ComponentBuilder components = new ComponentBuilder()
            .WithSelectMenu(configSetting)
            .WithButton(new ButtonBuilder("Go back", "user.profile.main", ButtonStyle.Primary));

        await ModifyOriginalResponseAsync(x =>
        {
            x.Content = message;
            x.Components = components.Build();
        });
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