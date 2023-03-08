using BaseBotService.Interfaces;
using BaseBotService.Extensions;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BaseBotService.Modules;

internal class UsersModule
{
    private readonly IActivityPointsService _activityPoints;

    public UsersModule(IActivityPointsService activityPoints)
    {
        _activityPoints = activityPoints;
    }

    /// <summary>
    /// Returns info about the current user, or the user parameter, if one is passed.
    /// </summary>
    /// <param name="user" isRequired="false">The users who's information you want to see.</param>
    internal async Task UserinfoCommandAsync(SocketSlashCommand cmd)
    {
        object targetUser = cmd.Data.Options.FirstOrDefault()?.Value ?? cmd.User;
        if (targetUser is IGuildUser guildUser)
        {
            await cmd.RespondAsync(embed: GenerateGuildUserInfo(cmd.User, guildUser).Build());
            return;
        }

        await cmd.RespondAsync(text: "You can only use this command on a server.", ephemeral: true);
    }

    private EmbedBuilder GenerateGuildUserInfo(SocketUser caller, IGuildUser user)
    {
        return new EmbedBuilder()
            .WithTitle($"{user.DisplayName} on {user.Guild.Name}")
            .WithAuthor(caller)
            .WithFields(new List<EmbedFieldBuilder>() {
            new EmbedFieldBuilder()
                .WithName("Name")
                .WithValue($"{user} {(user.IsBot ? "🤖" : string.Empty)}{(user.IsWebhook ? "🪝" : string.Empty)}")
                .WithIsInline(true),
            new EmbedFieldBuilder()
                .WithName("Created at")
                .WithValue(user.CreatedAt.ToDiscordTimestamp())
                .WithIsInline(true),
            new EmbedFieldBuilder()
                .WithName("Joined at")
                .WithValue(user.JoinedAt?.ToDiscordTimestamp())
                .WithIsInline(true)
            })
            .WithFieldIf(!user.IsBot && !user.IsWebhook,
            new EmbedFieldBuilder()
                .WithName("Last active")
                .WithValue(_activityPoints.GetLastActive(user.GuildId, user.Id).ToDiscordTimestamp())
                .WithIsInline(true))
            .WithFieldIf(!user.IsBot && !user.IsWebhook,
            new EmbedFieldBuilder()
                .WithName("Server points")
                .WithValue(_activityPoints.GetActivityPoints(user.GuildId, user.Id).ToString("N0", CultureInfo.InvariantCulture))
                .WithIsInline(true))
            .WithFields(new List<EmbedFieldBuilder>() {
            new EmbedFieldBuilder()
                .WithName("Roles")
                .WithValue(string.Join(", ", ((SocketGuildUser)user).Roles.Where(x => !x.IsEveryone).Select(x => x.Mention))),
            new EmbedFieldBuilder()
                .WithName("Permissions")
                .WithValue(string.Join(", ", Enum.GetValues(typeof(GuildPermission))
                    .Cast<GuildPermission>()
                    .Where(user.GuildPermissions.Has)
                    .Select(p => Regex.Replace(p.ToString(), "([a-z])([A-Z])", "$1 $2"))))
            })
            .WithThumbnailUrl(user.GetDisplayAvatarUrl())
            .WithColor(Color.LightOrange)
            .WithCurrentTimestamp();
    }

    /// <summary>
    /// Lists all roles of a user.
    /// </summary>
    /// <param name="user" isRequired="false">The users who's roles you want to be listed.</param>
    internal async Task ListRoleCommandAsync(SocketSlashCommand cmd)
    {
        SocketGuildUser? listUser = cmd.Data.Options.SingleOrDefault(o => o.Name.Equals("user", StringComparison.InvariantCultureIgnoreCase))?.Value as SocketGuildUser ?? cmd.User as SocketGuildUser;
        if (listUser == null)
        {
            await cmd.RespondAsync("This command does not work in DMs.", ephemeral: true);
            return;
        }

        string roleList = string.Join(",\n", listUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

        var response = new EmbedBuilder()
            .WithAuthor(listUser.ToString(), listUser.GetAvatarUrl() ?? listUser.GetDefaultAvatarUrl())
            .WithTitle($"Roles in {listUser.Guild.Name}")
            .WithDescription(roleList)
            .WithColor(Color.Green)
            .WithCurrentTimestamp();

        await cmd.RespondAsync(embed: response.Build());
    }
}