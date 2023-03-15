using BaseBotService.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;

namespace BaseBotService.Modules;
public abstract class BaseModule : InteractionModuleBase<SocketInteractionContext>
{
    // Dependencies can be accessed through Property injection,
    // public properties with public setters will be set by the service provider
    public ILogger Logger { get; set; } = null!;
    public SocketUser Caller => Context.Interaction.User;
    public SocketSelfUser BotUser => Context.Client.CurrentUser;
    public InteractionService Commands { get; set; } = null!;
    public IAssemblyService AssemblyService { get; set; } = null!;
    public IEnvironmentService EnvironmentService { get; set; } = null!;
    public IPersistenceService PersistenceService { get; set; } = null!;
    public EmbedBuilder GetEmbedBuilder() => new()
    {
        Author = new EmbedAuthorBuilder
        {
            Name = Caller.Username,
            IconUrl = Caller.GetAvatarUrl()
        },
        Color = Color.Gold,
        Timestamp = DateTimeOffset.UtcNow,
        Footer = new EmbedFooterBuilder
        {
            IconUrl = BotUser.GetAvatarUrl(),
            Text = $"Honeycomb v{AssemblyService.Version} ({EnvironmentService.EnvironmentName})"
        }
    };

}
