using BaseBotService.Commands.Enums;
using BaseBotService.Commands.Interfaces;
using BaseBotService.Commands.Modals;
using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Enums;
using BaseBotService.Utilities.Extensions;
using Discord.WebSocket;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseBotService.Commands;

[Group("user", "The user management module of Honeycomb.")]
public class UserModule : BaseModule
{
    private readonly ITranslationService _translationService;
    private readonly IEngagementService _engagementService;
    private readonly IMemberRepository _memberRepository;

    public UserModule(ILogger logger, ITranslationService translationService, IEngagementService engagementService, IMemberRepository memberRepository)
    {
        Logger = logger.ForContext<UserModule>();
        _translationService = translationService;
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

        ComponentBuilder configBtn = new ComponentBuilder()
            .WithButton(
                _translationService.GetAttrString("profile-config", "button"),
                "profile.config",
                ButtonStyle.Primary,
                Emoji.Parse(_translationService.GetAttrString("profile-config", "emoji")));

        await RespondOrFollowupAsync(embed: GetUserProfileEmbed(user, false).Build(), components: configBtn.Build(), ephemeral: false);
    }

    [SlashCommand("config", "Change the settings of your global Honeycomb profile.")]
    [ComponentInteraction("profile.config", ignoreGroupNames: true)]
    public async Task ConfigProfileAsync()
    {
        await RespondOrFollowupInDMAsync(_translationService.GetString("profile-config"), components: ShowUserConfigMenu().Build());
    }

    private ComponentBuilder ShowUserConfigMenu()
    {
        var userConfigMenu = new SelectMenuBuilder()
            .WithPlaceholder(_translationService.GetString("profile-config"))
            .WithCustomId("user.profile.config")
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOptionsFromEnum<UserConfigs>(-1, e => e.GetUserSettingsName(_translationService));

        return new ComponentBuilder()
            .WithSelectMenu(userConfigMenu)
            .WithButton(new ButtonBuilder(_translationService.GetString("button-close"), "user.profile.close", ButtonStyle.Primary));
    }

    [ComponentInteraction("user.profile.close", ignoreGroupNames: true)]
    public async Task CloseUserProfileAsync()
    {
        await DeferAsync();
        SocketMessageComponent component = (SocketMessageComponent)Context.Interaction;
        await component.ModifyOriginalResponseAsync(x =>
        {
            x.Content = _translationService.GetString("profile-saved");
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
            x.Content = _translationService.GetString("profile-config");
            x.Components = ShowUserConfigMenu().Build();
        });
    }

    [ComponentInteraction("user.profile.save.country", ignoreGroupNames: true)]
    public async Task SaveProfileCountryAsync(string[] selections)
    {
        Countries selection = Enum.Parse<Countries>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true)!;

        member.Country = selection;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.save.gender", ignoreGroupNames: true)]
    public async Task SaveProfileGenderIdentityAsync(string[] selections)
    {
        GenderIdentity selection = Enum.Parse<GenderIdentity>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true)!;

        member.GenderIdentity = selection;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.save.timezone", ignoreGroupNames: true)]
    public async Task SaveProfileTimezoneAsync(string[] selections)
    {
        Timezone selection = Enum.Parse<Timezone>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true)!;

        member.Timezone = selection;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ModalInteraction("user.profile.save.birthday", ignoreGroupNames: true)]
    public async Task SaveProfileBirthday(UserProfileSaveBirthdayModal data)
    {
        if (data?.Validate() == true)
        {
            MemberHC member = _memberRepository.GetUser(Caller.Id, true)!;
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

        MemberHC member = _memberRepository.GetUser(Caller.Id, true)!;

        member.Languages = selectedLanguages;
        _memberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.config", ignoreGroupNames: true)]
    public async Task UserProfileConfigAsync(string[] selections)
    {
        UserConfigs selection = Enum.Parse<UserConfigs>(selections.FirstOrDefault() ?? "0");
        MemberHC member = _memberRepository.GetUser(Caller.Id, true)!;

        SelectMenuBuilder configSetting = new();
        string message = string.Empty;

        switch (selection)
        {
            case UserConfigs.Country:
                configSetting.WithPlaceholder(_translationService.GetAttrString("profile-config", "country"))
                    .WithCustomId("user.profile.save.country")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Countries>((int)member.Country, e => e.GetCountryNameWithFlag(_translationService));
                message = _translationService.GetAttrString("profile-config", "country");
                break;
            case UserConfigs.Languages:
                configSetting.WithPlaceholder(_translationService.GetAttrString("profile-config", "languages"))
                    .WithCustomId("user.profile.save.languages")
                    .WithMinValues(1)
                    .WithMaxValues(4)
                    .AddOptionsFromEnum<Languages>((int)member.Languages, e => e.GetFlaggedLanguageNames(_translationService));
                message = _translationService.GetAttrString("profile-config", "languages");
                break;
            case UserConfigs.GenderIdentity:
                configSetting.WithPlaceholder(_translationService.GetAttrString("profile-config", "gender"))
                    .WithCustomId("user.profile.save.gender")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<GenderIdentity>((int)member.GenderIdentity, e => e.GetFlaggedGenderName(_translationService));
                message = _translationService.GetAttrString("profile-config", "gender");
                break;
            case UserConfigs.Timezone:
                configSetting.WithPlaceholder(_translationService.GetAttrString("profile-config", "timezone"))
                    .WithCustomId("user.profile.save.timezone")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Timezone>((int)member.Timezone, e => e.GetNameWithOffset());
                message = _translationService.GetAttrString("profile-config", "timezone");
                break;
            case UserConfigs.Birthday:
                var mb = new ModalBuilder()
                    .WithTitle(_translationService.GetString("profile-birthday"))
                    .WithCustomId("user.profile.save.birthday")
                    .AddTextInput(_translationService.GetAttrString("profile-birthday", "day"), "day",
                        placeholder: _translationService.GetAttrString("profile-birthday", "day-placeholder"),
                        maxLength: 2, value: member.Birthday?.Day.ToString("D2"))
                    .AddTextInput(_translationService.GetAttrString("profile-birthday", "month"), "month",
                        placeholder: _translationService.GetAttrString("profile-birthday", "month-placeholder"),
                        maxLength: 2, value: member.Birthday?.Month.ToString("D2"))
                    .AddTextInput(
                        _translationService.GetAttrString("profile-birthday", "year"), "year",
                        placeholder: _translationService.GetAttrString("profile-birthday", "year-placeholder",
                            TranslationHelper.Arguments("exampleYear", DateTime.UtcNow.Year - 18)),
                        maxLength: 4, required: false,
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
            .WithButton(new ButtonBuilder(_translationService.GetString("button-back"), "user.profile.main", ButtonStyle.Primary));

        await ModifyOriginalResponseAsync(x =>
        {
            x.Content = message;
            x.Components = components.Build();
        });
    }

    internal double GetActivityScore(IGuildUser user, IGuildUser bot)
    {
        const double averageOnlineHours = 4;
        const double scalingFactor = averageOnlineHours / 24;

        DateTimeOffset? userJoined = user?.JoinedAt;
        DateTimeOffset? botJoined = bot?.JoinedAt;
        if (!userJoined.HasValue || !botJoined.HasValue)
        {
            return 0;
        }

        DateTimeOffset startCountingDate = userJoined.Value > botJoined.Value ? userJoined.Value : botJoined.Value;
        double daysCounting = (DateTime.UtcNow - startCountingDate).TotalDays;
        double maxPoints = daysCounting * _engagementService.MaxPointsPerDay;
        double scaledMaxPoints = maxPoints * scalingFactor;
        Logger.Debug($"User {user!.Id} has been in the guild for {daysCounting} days, which would result in {maxPoints} points. The scaling factor is {scalingFactor}, so the maximum points are {scaledMaxPoints}.");
        if (scaledMaxPoints == 0)
        {
            return 0;
        }
        return Math.Min(100, _engagementService.GetActivityPoints(user.GuildId, user.Id) / scaledMaxPoints * 100);
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
                Name = _translationService.GetAttrString("profile", "name"),
                Value = $"{user} {(user.IsBot ? UnicodeEmojiHelper.robotFace : string.Empty)}{(user.IsWebhook ? UnicodeEmojiHelper.satelliteAntenna : string.Empty)}"
            },
            new EmbedFieldBuilder
            {
                Name = _translationService.GetAttrString("profile", "created"),
                Value = $"{user.CreatedAt.ToDiscordTimestamp(_translationService, DiscordTimestampFormat.ShortDateTime)}\n({user.CreatedAt.ToDiscordTimestamp(_translationService, DiscordTimestampFormat.RelativeTime)})",
                IsInline = true
            }
        };

        MemberHC? member = _memberRepository.GetUser(user.Id);
        if (member != null)
        {
            fields.AddRange(new[] {
                new EmbedFieldBuilder
                {
                    Name = _translationService.GetAttrString("profile", "country"),
                    Value = member.Country.GetCountryNameWithFlag(_translationService),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = _translationService.GetAttrString("profile", "languages"),
                    Value = member.Languages.GetFlaggedLanguageNames(_translationService),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = _translationService.GetAttrString("profile", "gender"),
                    Value = member.GenderIdentity.GetFlaggedGenderName(_translationService),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = _translationService.GetAttrString("profile", "timezone"),
                    Value = member.Timezone.GetNameWithOffset(),
                    IsInline = true
                }
            });

            if (member.Birthday.HasValue)
            {
                fields.Add(
                    new EmbedFieldBuilder
                    {
                        Name = _translationService.GetAttrString("profile", "birthday"),
                        IsInline = true,
                        Value = member.Birthday.Value.Year > 1
                            ? $"{member.Birthday.Value.ToDiscordTimestamp(_translationService, DiscordTimestampFormat.ShortDate)} ({member.Birthday.Value.GetAge()})"
                            : $"{member.Birthday.Value.GetDayAndMonth(member.Country)}"
                    }
                );
            }
        }

        if (user is IGuildUser gUser)
        {
            fields.Add(
            new EmbedFieldBuilder
            {
                Name = _translationService.GetAttrString("profile", "joined"),
                Value = $"{gUser.JoinedAt?.ToDiscordTimestamp(_translationService, DiscordTimestampFormat.ShortDateTime)}\n({gUser.JoinedAt?.ToDiscordTimestamp(_translationService, DiscordTimestampFormat.RelativeTime)})",
                IsInline = true
            });

            if (!user.IsBot && !user.IsWebhook)
            {
                const int activityMaxSteps = 12;
                double userActivityScore = GetActivityScore(gUser, Context.Guild.CurrentUser);
                int userActivityProgress = (int)(userActivityScore / (100 / activityMaxSteps));
                bool isTooNewForProgress = gUser.JoinedAt!.Value.AddDays(1) > DateTime.UtcNow;
                StringBuilder progressBar = new();
                progressBar
                    .Append(UnicodeEmojiHelper.greenSquare.Repeat(userActivityProgress))
                    .Append(UnicodeEmojiHelper.whiteSquare.Repeat(activityMaxSteps - userActivityProgress))
                    .Append(' ')
                    .AppendFormat("{0:F2}", userActivityScore)
                    .AppendLine("%")
                    .Append(_translationService.GetAttrString(
                        "profile", "activity-rating",
                        TranslationHelper.Arguments("score", userActivityProgress)
                        )
                    );
                Logger.Debug($"Calculating Activity Progress for User ID: {gUser.Id} on Guild ID: {Context.Guild.Id} {Environment.NewLine}Max Steps: {activityMaxSteps}, Activity Score: {userActivityScore}, Progress: {userActivityProgress}");

                fields.AddRange(new[] {
                    new EmbedFieldBuilder
                    {
                        Name = _translationService.GetAttrString("profile", "active"),
                        Value = $"{_engagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(_translationService, DiscordTimestampFormat.ShortDateTime)}\n({_engagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(_translationService, DiscordTimestampFormat.RelativeTime)})",
                        IsInline = true
                    },
                    new EmbedFieldBuilder
                    {
                        Name = _translationService.GetAttrString("profile", "activity"),
                        Value = isTooNewForProgress ? _translationService.GetAttrString("profile", "activity-calc") : progressBar.ToString()
                    }
                });
            }

            IEnumerable<string> roleMentions = ((SocketGuildUser)gUser).Roles.Where(x => !x.IsEveryone).Select(x => x.Mention);
            fields.Add(
                new EmbedFieldBuilder
                {
                    Name = _translationService.GetAttrString("profile", "roles"),
                    Value = roleMentions.Any() ? string.Join(", ", roleMentions) : _translationService.GetAttrString("profile", "roles-none")
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
                    Name = _translationService.GetAttrString("profile", "permissions"),
                    Value = permissionNames.Any() ? string.Join(", ", permissionNames) : _translationService.GetAttrString("profile", "permissions-none")
                });
            }
            result.Title = _translationService
                .GetString("profile",
                TranslationHelper.Arguments("username", gUser.DisplayName, "guildname", gUser.Guild.Name));
            result.ThumbnailUrl = gUser.GetDisplayAvatarUrl();
        }

        _ = result.WithFields(fields);

        return result;
    }
}