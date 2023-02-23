using Discord;
using Discord.Commands;
using System.Reflection;

namespace BaseBotService.Modules;

public class InfoModule : ModuleBase<SocketCommandContext>
{
    [Command("info")]
    [Summary("Returns the basic information of the bot.")]
    public Task InfoAsync()
    {
        // Get bot name and version from assembly information
        var assembly = Assembly.GetExecutingAssembly();
        var name = assembly.GetName().Name;
        var version = assembly.GetName().Version;

        // Create embedded message with bot information
        var embed = new EmbedBuilder()
            .WithTitle(name)
            .WithAuthor("anheledir.NET")
            .WithDescription($"This is a simple discord bot for artists")
            .WithFooter($"Version {version}")
            .WithColor(Color.DarkPurple);

        // Send embedded message to default channel
        ReplyAsync(embed: embed.Build());
        return Task.CompletedTask;
    }
}