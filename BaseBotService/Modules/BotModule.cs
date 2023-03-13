using BaseBotService.Interfaces;
using Discord;
using Discord.Interactions;

namespace BaseBotService.Modules;

[Group("bot", "The main bot information module of Honeycomb.")]
[EnabledInDm(true)]
public class BotModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
    public InteractionService Commands { get; set; } = null!;
    public IAssemblyService AssemblyService { get; set; } = null!;
    public IEnvironmentService EnvironmentService { get; set; } = null!;

    [SlashCommand("info", "Returns information like runtime and current version of this Honeycomb bot instance.")]
    public async Task InfoCommandAsync()
    {
        // Create embedded message with bot information
        var response = new EmbedBuilder()
            .WithTitle(AssemblyService.Name)
            .WithAuthor("github.com/anheledir")
            .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
            .WithUrl("https://anheledir.github.io/Honeycomb/")
            .WithDescription($"Honeycomb is a Discord bot designed to provide artists with some useful functions to enhance their experience on Discord. With its features, artists can create a portfolio, display random entries from it, manage a commission price list, and keep track of their commission queue.")
            .WithFields(
                new EmbedFieldBuilder()
                .WithName("Uptime")
                .WithValue(EnvironmentService.GetUptime()),
                new EmbedFieldBuilder()
                .WithName("Total servers")
                .WithValue(Context.Client.Guilds.Count)
                .WithIsInline(true)
            )
            .WithFooter($"Version {AssemblyService.Version} ({EnvironmentService.EnvironmentName})")
            .WithColor(Color.DarkPurple)
            .WithCurrentTimestamp();
        await RespondAsync(embed: response.Build());
    }

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task GreetUserAsync()
        => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

}