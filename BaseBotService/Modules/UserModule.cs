using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BaseBotService.Modules;

[Group("user", "The user management module of Honeycomb.")]
[EnabledInDm(false)]
public class UserModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public IEngagementService Engagement { get; set; } = null!;

    [UserCommand("Honeycomb Infos")]
    public async Task UserInfoCommandAsync(IUser user) => await UserinfoCommandAsync(user, false);

    [SlashCommand("info", "Returns info about the current user, or the user parameter, if one is passed.")]
    public async Task UserinfoCommandAsync(
        [Summary(description: "The users who's information you want to see, leave empty for yourself.")]
        IUser? user = null,
        [Summary(description: "Whether to include the users permissions or not.")]
        bool includePermissions = false
        )
    {
        user ??= Context.User;
        await RespondAsync(embed: GenerateGuildUserInfo(Context.Interaction.User, (IGuildUser)user, includePermissions).Build());
    }

    private EmbedBuilder GenerateGuildUserInfo(SocketUser caller, IGuildUser user, bool includePermissions)
    {
        var roleMentions = ((SocketGuildUser)user).Roles.Where(x => !x.IsEveryone).Select(x => x.Mention);

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
        },
        new EmbedFieldBuilder
        {
            Name = "Joined at",
            Value = $"{user.JoinedAt?.ToDiscordTimestamp(DiscordTimestampFormat.ShortDateTime)}\n({user.JoinedAt?.ToDiscordTimestamp(DiscordTimestampFormat.RelativeTime)})",
            IsInline = true
        }
    };
        if (!user.IsBot && !user.IsWebhook)
        {
            fields.AddRange(new[] {
                new EmbedFieldBuilder
                {
                    Name = "Last active",
                    Value = $"{Engagement.GetLastActive(user.GuildId, user.Id).ToDiscordTimestamp(DiscordTimestampFormat.ShortDateTime)}\n({Engagement.GetLastActive(user.GuildId, user.Id).ToDiscordTimestamp(DiscordTimestampFormat.RelativeTime)})",
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Server points",
                    Value = Engagement.GetActivityPoints(user.GuildId, user.Id).ToString("N0", CultureInfo.InvariantCulture)
                }
            });
        }
        fields.Add(
            new EmbedFieldBuilder
            {
                Name = "Roles",
                Value = string.Join(", ", roleMentions)
            });

        if (includePermissions)
        {
            IEnumerable<string> permissionNames = Enum.GetValues(typeof(GuildPermission))
                                    .Cast<GuildPermission>()
                                    .Where(user.GuildPermissions.Has)
                                    .Select(p => Regex.Replace(p.ToString(), "([a-z])([A-Z])", "$1 $2"));

            fields.Add(
            new EmbedFieldBuilder
            {
                Name = "Permissions",
                Value = string.Join(", ", permissionNames)
            });
        }

        return new EmbedBuilder
        {
            Title = $"{user.DisplayName} @ {user.Guild.Name}",
            Author = new EmbedAuthorBuilder
            {
                Name = caller.Username,
                IconUrl = caller.GetAvatarUrl()
            },
            Fields = fields,
            ThumbnailUrl = user.GetDisplayAvatarUrl(),
            Color = Color.LightOrange,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}