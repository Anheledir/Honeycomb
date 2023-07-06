using BaseBotService.Core.Messages;
using Discord.WebSocket;
using System.Reflection;

namespace BaseBotService.Core;

public class DiscordEventListener(
    ILogger logger,
    DiscordSocketClient client,
    IServiceProvider services,
    InteractionService handler,
    IMediator mediator)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S3604:Member initializer values should not be redundant", Justification = "False-positive.")]
    private readonly CancellationToken _cancellationToken = new CancellationTokenSource().Token;

    public async Task StartAsync()
    {
        logger.Information("Starting Discord event listener.");

        client.MessageReceived += (socketMessage) => mediator.Publish(new MessageReceivedNotification(socketMessage), _cancellationToken);
        client.Log += (msg) => mediator.Publish(new LogNotification(msg), _cancellationToken);
        client.ReactionAdded += (cache, channel, reaction) => mediator.Publish(new ReactionAddedNotification(cache, channel, reaction), _cancellationToken);
        client.Ready += () => mediator.Publish(new ClientReadyNotification(), _cancellationToken);
        client.Disconnected += (ex) => mediator.Publish(new ClientDisconnectedNotification(ex), _cancellationToken);
        client.InteractionCreated += (interaction) => mediator.Publish(new InteractionCreatedNotification(interaction), _cancellationToken);
        client.JoinedGuild += (guild) => mediator.Publish(new JoinedGuildNotification(guild), _cancellationToken);
        client.LeftGuild += (guild) => mediator.Publish(new LeftGuildNotification(guild), _cancellationToken);
        client.UserJoined += (user) => mediator.Publish(new UserJoinedNotification(user), _cancellationToken);
        handler.Log += (msg) => mediator.Publish(new LogNotification(msg), _cancellationToken);

        _ = await handler.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        logger.Information($"Found {handler.Modules.Count} modules.");

        await Task.CompletedTask;
    }
}