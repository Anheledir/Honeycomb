using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BaseBotService.Modules;

[Group("users", "The user management module of Honeycomb.")]
[EnabledInDm(false)]
public class UsersModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public IEngagementService Engagement { get; set; } = null!;

    [SlashCommand("info", "Returns info about the current user, or the user parameter, if one is passed.")]
    [UserCommand("Honeycomb Infos")]
    public async Task UserinfoCommandAsync(
        [Summary(description: "The users who's information you want to see, leave empty for yourself.")]
        IUser? user = null
        )
    {
        user ??= Context.User;
        await RespondAsync(embed: GenerateGuildUserInfo(Context.Interaction.User, (IGuildUser)user).Build());
    }

    private EmbedBuilder GenerateGuildUserInfo(SocketUser caller, IGuildUser user)
    {
        var roleMentions = ((SocketGuildUser)user).Roles.Where(x => !x.IsEveryone).Select(x => x.Mention);
        var permissionNames = Enum.GetValues(typeof(GuildPermission))
                                .Cast<GuildPermission>()
                                .Where(user.GuildPermissions.Has)
                                .Select(p => Regex.Replace(p.ToString(), "([a-z])([A-Z])", "$1 $2"));

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
        },
        new EmbedFieldBuilder
        {
            Name = "Last active",
            Value = user.IsBot || user.IsWebhook ? null : $"{Engagement.GetLastActive(user.GuildId, user.Id).ToDiscordTimestamp(DiscordTimestampFormat.ShortDateTime)}\n({user.JoinedAt?.ToDiscordTimestamp(DiscordTimestampFormat.RelativeTime)})",
            IsInline = true
        },
        new EmbedFieldBuilder
        {
            Name = "Server points",
            Value = user.IsBot || user.IsWebhook ? null : Engagement.GetActivityPoints(user.GuildId, user.Id).ToString("N0", CultureInfo.InvariantCulture)
        },
        new EmbedFieldBuilder
        {
            Name = "Roles",
            Value = string.Join(", ", roleMentions)
        },
        new EmbedFieldBuilder
        {
            Name = "Permissions",
            Value = string.Join(", ", permissionNames)
        }
    };

        return new EmbedBuilder
        {
            Title = $"{user.DisplayName} @ {user.Guild.Name}",
            Author = new EmbedAuthorBuilder { Name = caller.Username, IconUrl = caller.GetAvatarUrl() },
            Fields = fields,
            ThumbnailUrl = user.GetDisplayAvatarUrl(),
            Color = Color.LightOrange,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}