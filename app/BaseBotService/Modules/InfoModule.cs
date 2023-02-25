using BaseBotService.Interfaces;
using Discord;
using Discord.WebSocket;

namespace BaseBotService.Modules;

internal class InfoModule
{
    private readonly IAssemblyService _assemblyService;

    public InfoModule(IAssemblyService assemblyService)
    {
        _assemblyService = assemblyService;
    }
    internal async Task InfoCommandAsync(SocketSlashCommand cmd)
    {
        // Create embedded message with bot information
        var response = new EmbedBuilder()
            .WithTitle(_assemblyService.Name)
            .WithAuthor("github.com/anheledir")
            .WithDescription($"This is a simple discord bot to provide useful tools for artists.")
            .WithFooter($"Version {_assemblyService.Version}")
            .WithColor(Color.DarkPurple)
            .WithCurrentTimestamp();

        await cmd.RespondAsync(embed: response.Build());
    }
}