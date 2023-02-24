using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace BaseBotService.Modules;

internal class UsersModule : InteractionModuleBase
{
    [SlashCommand("user-info", "Returns info about the current user, or the user parameter, if one is passed.")]
    internal async Task UserinfoCommandAsync(SocketSlashCommand cmd)
    {
        IUser userInfo = (IUser)cmd.Data.Options.FirstOrDefault()?.Value ?? Context.Interaction.User;

        var msg = new EmbedBuilder()
            .WithTitle("UserInfo")
            .WithAuthor(Context.Interaction.User)
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

        await ReplyAsync(embed: msg.Build());
    }

    [SlashCommand("user-roles", "Lists all roles of a user.")]
    internal async Task ListRoleCommandAsync(SocketSlashCommand cmd)
    {
        // extract the user parameter from the command
        // since we only have one option and it's required, we can just use the first option
        SocketGuildUser guildUser = (SocketGuildUser)cmd.Data.Options.FirstOrDefault()?.Value ?? (SocketGuildUser)Context.Interaction.User;

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