using BaseBotService.Commands.Interfaces;
using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;

public class EngagementServiceHandler : INotificationHandler<MessageReceivedNotification>
{
    private readonly ILogger _logger;
    private readonly IEngagementService _engagementService;
    private readonly DiscordSocketClient _client;

    public EngagementServiceHandler(ILogger logger, IEngagementService engagementService, DiscordSocketClient client)
    {
        _logger = logger.ForContext<EngagementServiceHandler>();
        _engagementService = engagementService;
        _client = client;
    }

    public Task Handle(MessageReceivedNotification arg, CancellationToken cancellationToken)
    {
        // ignore own messages
        if (arg.Message.Author.Id == _client.CurrentUser.Id)
        {
            return Task.CompletedTask;
        }

        // log DMs from users
        if (arg.Message.Channel is IDMChannel && !arg.Message.Author.IsBot)
        {
            _logger.Debug($"DM from [{arg.Message.Author.Username}#{arg.Message.Author.Discriminator}]: {arg.Message.CleanContent}");
        }

        // log messages from bots and webhooks
        if (arg.Message.Author.IsBot || arg.Message.Author.IsWebhook)
        {
            _logger.Debug($"Message from [{arg.Message.Author.Username}#{arg.Message.Author.Discriminator}]: isBot = {arg.Message.Author.IsBot}, isWebhook = {arg.Message.Author.IsWebhook}");
        }

        ulong userId = arg.Message.Author.Id;
        ulong guildId = arg.Message.Channel is IGuildChannel guildChannel ? guildChannel.GuildId : 0;
        _logger.Debug($"Message in guild {guildId} by user {userId}");

        // only measure activity in guilds
        if (guildId > 0)
        {
            _ = _engagementService.AddActivityTick(guildId, userId);
        }

        return Task.CompletedTask;
    }
}