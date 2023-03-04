using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace BaseBotService.Modules;

internal class UsersModule
{
    /// <summary>
    /// Returns info about the current user, or the user parameter, if one is passed.
    /// </summary>
    /// <param name="user" isRequired="false">The users who's information you want to see.</param>
    internal async Task UserinfoCommandAsync(SocketSlashCommand cmd)
    {
        IUser userInfo = cmd.Data.Options.FirstOrDefault()?.Value as IUser ?? cmd.User;

        var msg = new EmbedBuilder()
            .WithTitle("UserInfo")
            .WithAuthor(cmd.User)
            .WithFields(new List<EmbedFieldBuilder>() {
            new EmbedFieldBuilder()
                .WithName("id")
                .WithValue(userInfo.Id),
            new EmbedFieldBuilder()
                .WithName("name")
                .WithValue($"{userInfo.Username}#{userInfo.Discriminator}")
                .WithIsInline(true),
            new EmbedFieldBuilder()
                .WithName("status")
                .WithValue(userInfo.Status)
                .WithIsInline(true),
            new EmbedFieldBuilder()
                .WithName("created at")
                .WithValue(userInfo.CreatedAt)
            })
            .WithThumbnailUrl(userInfo.GetAvatarUrl())
            .WithColor(Color.LightOrange)
            .WithCurrentTimestamp();

        await cmd.RespondAsync(embed: msg.Build());
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