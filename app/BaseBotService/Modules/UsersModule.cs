using Discord.Commands;
using Discord.WebSocket;

namespace BaseBotService.Modules;

public class UsersModule : ModuleBase<SocketCommandContext>
{
    /// <summary>
    /// Returns info about the current user, or <paramref name="user"/>, if one was passed.
    /// </summary>
    /// <param name="user">(optional) The user to return information for.</param>
    /// <example>
    /// ~users userinfo --> foxbot#0282
    /// ~users userinfo @Khionu --> Khionu#8708
    /// ~users userinfo Khionu#8708 --> Khionu#8708
    /// ~users userinfo Khionu --> Khionu#8708
    /// ~users user 96642168176807936 --> Khionu#8708
    /// ~users whois 96642168176807936 --> Khionu#8708
    /// </example>
    [Command("userinfo")]
    [Summary
    ("Returns info about the current user, or the user parameter, if one is passed.")]
    [Alias("user", "whois")]
    public async Task UserInfoAsync(
        [Summary("The (optional) user to get info from")]
        SocketUser? user = null)
    {
        var userInfo = user ?? Context.Client.CurrentUser;
        await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
}