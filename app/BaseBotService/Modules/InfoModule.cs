using BaseBotService.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace BaseBotService.Modules;

internal class InfoModule : InteractionModuleBase
{
    private readonly AssemblyService _assemblyService;

    internal InfoModule(AssemblyService assemblyService)
    {
        _assemblyService = assemblyService;
    }

    [SlashCommand("info", "Returns the basic information of the bot.")]
    internal async Task InfoCommandAsync()
    {
        // Create embedded message with bot information
        var response = new EmbedBuilder()
            .WithTitle(_assemblyService.Name)
            .WithAuthor("github.com/anheledir")
            .WithDescription($"This is a simple discord bot for artists")
            .WithFooter($"Version {_assemblyService.Version}")
            .WithColor(Color.DarkPurple)
            .WithCurrentTimestamp();

        await RespondAsync(embed: response.Build());
    }
}