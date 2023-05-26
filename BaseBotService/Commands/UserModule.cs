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
public class UserModule(ITranslationService TranslationService, IEngagementService EngagementService, IMemberRepository MemberRepository) : BaseModule
{
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
                TranslationService.GetAttrString("profile-config", "button"),
                "profile.config",
                ButtonStyle.Primary,
                Emoji.Parse(TranslationService.GetAttrString("profile-config", "emoji")));

        await RespondOrFollowupAsync(embed: GetUserProfileEmbed(user, false).Build(), components: configBtn.Build(), ephemeral: false);
    }

    [SlashCommand("config", "Change the settings of your global Honeycomb profile.")]
    [ComponentInteraction("profile.config", ignoreGroupNames: true)]
    public async Task ConfigProfileAsync()
    {
        await RespondOrFollowupInDMAsync(TranslationService.GetString("profile-config"), components: ShowUserConfigMenu().Build());
    }

    private ComponentBuilder ShowUserConfigMenu()
    {
        var userConfigMenu = new SelectMenuBuilder()
            .WithPlaceholder(TranslationService.GetString("profile-config"))
            .WithCustomId("user.profile.config")
            .WithMinValues(1)
            .WithMaxValues(1)
            .AddOptionsFromEnum<UserConfigs>(-1, e => e.GetUserSettingsName(TranslationService));

        return new ComponentBuilder()
            .WithSelectMenu(userConfigMenu)
            .WithButton(new ButtonBuilder(TranslationService.GetString("button-close"), "user.profile.close", ButtonStyle.Primary));
    }

    [ComponentInteraction("user.profile.close", ignoreGroupNames: true)]
    public async Task CloseUserProfileAsync()
    {
        await DeferAsync();
        SocketMessageComponent component = (SocketMessageComponent)Context.Interaction;
        await component.ModifyOriginalResponseAsync(x =>
        {
            x.Content = TranslationService.GetString("profile-saved");
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
            x.Content = TranslationService.GetString("profile-config");
            x.Components = ShowUserConfigMenu().Build();
        });
    }

    [ComponentInteraction("user.profile.save.country", ignoreGroupNames: true)]
    public async Task SaveProfileCountryAsync(string[] selections)
    {
        Countries selection = Enum.Parse<Countries>(selections.FirstOrDefault() ?? "0");
        MemberHC member = MemberRepository.GetUser(Caller.Id, true)!;

        member.Country = selection;
        MemberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.save.gender", ignoreGroupNames: true)]
    public async Task SaveProfileGenderIdentityAsync(string[] selections)
    {
        GenderIdentity selection = Enum.Parse<GenderIdentity>(selections.FirstOrDefault() ?? "0");
        MemberHC member = MemberRepository.GetUser(Caller.Id, true)!;

        member.GenderIdentity = selection;
        MemberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.save.timezone", ignoreGroupNames: true)]
    public async Task SaveProfileTimezoneAsync(string[] selections)
    {
        Timezone selection = Enum.Parse<Timezone>(selections.FirstOrDefault() ?? "0");
        MemberHC member = MemberRepository.GetUser(Caller.Id, true)!;

        member.Timezone = selection;
        MemberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ModalInteraction("user.profile.save.birthday", ignoreGroupNames: true)]
    public async Task SaveProfileBirthday(UserProfileSaveBirthdayModal data)
    {
        if (data?.Validate() == true)
        {
            MemberHC member = MemberRepository.GetUser(Caller.Id, true)!;
            member.Birthday = data.GetBirthday();
            MemberRepository.UpdateUser(member);
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

        MemberHC member = MemberRepository.GetUser(Caller.Id, true)!;

        member.Languages = selectedLanguages;
        MemberRepository.UpdateUser(member);
        await DeferAsync();
    }

    [ComponentInteraction("user.profile.config", ignoreGroupNames: true)]
    public async Task UserProfileConfigAsync(string[] selections)
    {
        UserConfigs selection = Enum.Parse<UserConfigs>(selections.FirstOrDefault() ?? "0");
        MemberHC member = MemberRepository.GetUser(Caller.Id, true)!;

        SelectMenuBuilder configSetting = new();
        string message = string.Empty;

        switch (selection)
        {
            case UserConfigs.Country:
                configSetting.WithPlaceholder(TranslationService.GetAttrString("profile-config", "country"))
                    .WithCustomId("user.profile.save.country")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Countries>((int)member.Country, e => e.GetCountryNameWithFlag(TranslationService));
                message = TranslationService.GetAttrString("profile-config", "country");
                break;
            case UserConfigs.Languages:
                configSetting.WithPlaceholder(TranslationService.GetAttrString("profile-config", "languages"))
                    .WithCustomId("user.profile.save.languages")
                    .WithMinValues(1)
                    .WithMaxValues(4)
                    .AddOptionsFromEnum<Languages>((int)member.Languages, e => e.GetFlaggedLanguageNames(TranslationService));
                message = TranslationService.GetAttrString("profile-config", "languages");
                break;
            case UserConfigs.GenderIdentity:
                configSetting.WithPlaceholder(TranslationService.GetAttrString("profile-config", "gender"))
                    .WithCustomId("user.profile.save.gender")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<GenderIdentity>((int)member.GenderIdentity, e => e.GetFlaggedGenderName(TranslationService));
                message = TranslationService.GetAttrString("profile-config", "gender");
                break;
            case UserConfigs.Timezone:
                configSetting.WithPlaceholder(TranslationService.GetAttrString("profile-config", "timezone"))
                    .WithCustomId("user.profile.save.timezone")
                    .WithMinValues(1)
                    .WithMaxValues(1)
                    .AddOptionsFromEnum<Timezone>((int)member.Timezone, e => e.GetNameWithOffset());
                message = TranslationService.GetAttrString("profile-config", "timezone");
                break;
            case UserConfigs.Birthday:
                var mb = new ModalBuilder()
                    .WithTitle(TranslationService.GetString("profile-birthday"))
                    .WithCustomId("user.profile.save.birthday")
                    .AddTextInput(TranslationService.GetAttrString("profile-birthday", "day"), "day",
                        placeholder: TranslationService.GetAttrString("profile-birthday", "day-placeholder"),
                        maxLength: 2, value: member.Birthday?.Day.ToString("D2"))
                    .AddTextInput(TranslationService.GetAttrString("profile-birthday", "month"), "month",
                        placeholder: TranslationService.GetAttrString("profile-birthday", "month-placeholder"),
                        maxLength: 2, value: member.Birthday?.Month.ToString("D2"))
                    .AddTextInput(
                        TranslationService.GetAttrString("profile-birthday", "year"), "year",
                        placeholder: TranslationService.GetAttrString("profile-birthday", "year-placeholder",
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
            .WithButton(new ButtonBuilder(TranslationService.GetString("button-back"), "user.profile.main", ButtonStyle.Primary));

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
        double maxPoints = daysCounting * EngagementService.MaxPointsPerDay;
        double scaledMaxPoints = maxPoints * scalingFactor;
        Logger.Debug($"User {user!.Id} has been in the guild for {daysCounting} days, which would result in {maxPoints} points. The scaling factor is {scalingFactor}, so the maximum points are {scaledMaxPoints}.");
        return Math.Min(100, EngagementService.GetActivityPoints(user.GuildId, user.Id) / scaledMaxPoints * 100);
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
                Name = TranslationService.GetAttrString("profile", "name"),
                Value = $"{user} {(user.IsBot ? UnicodeEmojiHelper.robotFace : string.Empty)}{(user.IsWebhook ? UnicodeEmojiHelper.satelliteAntenna : string.Empty)}"
            },
            new EmbedFieldBuilder
            {
                Name = TranslationService.GetAttrString("profile", "created"),
                Value = $"{user.CreatedAt.ToDiscordTimestamp(TranslationService, DiscordTimestampFormat.ShortDateTime)}\n({user.CreatedAt.ToDiscordTimestamp(TranslationService, DiscordTimestampFormat.RelativeTime)})",
                IsInline = true
            }
        };

        MemberHC? member = MemberRepository.GetUser(user.Id);
        if (member != null)
        {
            fields.AddRange(new[] {
                new EmbedFieldBuilder
                {
                    Name = TranslationService.GetAttrString("profile", "country"),
                    Value = member.Country.GetCountryNameWithFlag(TranslationService),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = TranslationService.GetAttrString("profile", "languages"),
                    Value = member.Languages.GetFlaggedLanguageNames(TranslationService),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = TranslationService.GetAttrString("profile", "gender"),
                    Value = member.GenderIdentity.GetFlaggedGenderName(TranslationService),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = TranslationService.GetAttrString("profile", "timezone"),
                    Value = member.Timezone.GetNameWithOffset(),
                    IsInline = true
                }
            });

            if (member.Birthday.HasValue)
            {
                fields.Add(
                    new EmbedFieldBuilder
                    {
                        Name = TranslationService.GetAttrString("profile", "birthday"),
                        IsInline = true,
                        Value = member.Birthday.Value.Year > 1
                            ? $"{member.Birthday.Value.ToDiscordTimestamp(TranslationService, DiscordTimestampFormat.ShortDate)} ({member.Birthday.Value.GetAge()})"
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
                Name = TranslationService.GetAttrString("profile", "joined"),
                Value = $"{gUser.JoinedAt?.ToDiscordTimestamp(TranslationService, DiscordTimestampFormat.ShortDateTime)}\n({gUser.JoinedAt?.ToDiscordTimestamp(TranslationService, DiscordTimestampFormat.RelativeTime)})",
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
                    .Append(TranslationService.GetAttrString(
                        "profile", "activity-rating",
                        TranslationHelper.Arguments("score", userActivityProgress)
                        )
                    );
                Logger.Debug($"Calculating Activity Progress for User ID: {gUser.Id} on Guild ID: {Context.Guild.Id} {Environment.NewLine}Max Steps: {activityMaxSteps}, Activity Score: {userActivityScore}, Progress: {userActivityProgress}");

                fields.AddRange(new[] {
                    new EmbedFieldBuilder
                    {
                        Name = TranslationService.GetAttrString("profile", "active"),
                        Value = $"{EngagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(TranslationService, DiscordTimestampFormat.ShortDateTime)}\n({EngagementService.GetLastActive(gUser.GuildId, user.Id).ToDiscordTimestamp(TranslationService, DiscordTimestampFormat.RelativeTime)})",
                        IsInline = true
                    },
                    new EmbedFieldBuilder
                    {
                        Name = TranslationService.GetAttrString("profile", "activity"),
                        Value = isTooNewForProgress ? TranslationService.GetAttrString("profile", "activity-calc") : progressBar.ToString()
                    }
                });
            }

            IEnumerable<string> roleMentions = ((SocketGuildUser)gUser).Roles.Where(x => !x.IsEveryone).Select(x => x.Mention);
            fields.Add(
                new EmbedFieldBuilder
                {
                    Name = TranslationService.GetAttrString("profile", "roles"),
                    Value = roleMentions.Any() ? string.Join(", ", roleMentions) : TranslationService.GetAttrString("profile", "roles-none")
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
                    Name = TranslationService.GetAttrString("profile", "permissions"),
                    Value = permissionNames.Any() ? string.Join(", ", permissionNames) : TranslationService.GetAttrString("profile", "permissions-none")
                });
            }
            result.Title = TranslationService
                .GetString("profile",
                TranslationHelper.Arguments("username", gUser.DisplayName, "guildname", gUser.Guild.Name));
            result.ThumbnailUrl = gUser.GetDisplayAvatarUrl();
        }

        _ = result.WithFields(fields);

        return result;
    }
}