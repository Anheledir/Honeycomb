using Discord;
using Discord.WebSocket;

namespace BaseBotService.Modules;

internal class UsersModule
{
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

    internal async Task ListRoleCommandAsync(SocketSlashCommand cmd)
    {
        // extract the user parameter from the command
        // since we only have one option and it's required, we can just use the first option
        SocketGuildUser? guildUser = cmd.Data.Options.FirstOrDefault()?.Value as SocketGuildUser ?? cmd.User as SocketGuildUser;

        var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

        var response = new EmbedBuilder()
            .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
            .WithTitle("Roles")
            .WithDescription(roleList)
            .WithColor(Color.Green)
            .WithCurrentTimestamp();

        await cmd.RespondAsync(embed: response.Build());
    }
}