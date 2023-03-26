using BaseBotService.Core.Base;
using BaseBotService.Utilities.Attributes;

namespace BaseBotService.Commands;

[Group("bot", "The main bot information module of Honeycomb.")]
[EnabledInDm(true)]
public class BotModule : BaseModule
{
    public BotModule(ILogger logger)
    {
        Logger = logger.ForContext<BotModule>();
    }

    [SlashCommand("about", "Returns information like runtime and current version of this Honeycomb bot instance.")]
    public async Task AboutAsync()
    {
        // Create embedded message with bot information
        EmbedBuilder response = GetEmbedBuilder()
            .WithTitle(AssemblyService.Name)
            .WithThumbnailUrl(BotUser.GetAvatarUrl())
            .WithUrl("https://honeycombs.cloud/") // TODO(i18n): Move URL to localization resource
            .WithDescription("Honeycomb is a Discord bot designed to provide artists with some useful functions to enhance their experience on Discord. With its features, artists can create a portfolio, display random entries from it, manage a commission price list, and keep track of their commission queue. The bot is released under the MIT license on GitHub.")
            .WithFields(
                new EmbedFieldBuilder()
                .WithName("Uptime")
                .WithValue(EnvironmentService.GetUptime()),
                new EmbedFieldBuilder()
                .WithName("Total servers")
                .WithValue(Context.Client.Guilds.Count)
                .WithIsInline(true)
            )
            .WithColor(Color.DarkPurple);
        await FollowupAsync(embed: response.Build());
    }

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task PingAsync()
        => await FollowupAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

    [SlashCommand("documentation", "Sends a json-file via DM containing all command documentations.")]
    [RateLimit(1, 300)]
    public async Task DocumentationAsync()
    {
        if (!await CheckRateLimitAsync())
        {
            await RespondOrFollowupAsync(text: "You have to wait until you can use this again.");
            return;
        }
        await DeferAsync();

        string jsonString = Utilities.DocumentationUtility.GenerateDocumentationJson();
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
        using MemoryStream stream = new(jsonBytes);

        IDMChannel dm = await Caller.CreateDMChannelAsync();
        await dm.SendFileAsync(new FileAttachment(stream, $"honeycomb_v{AssemblyService.Version.Replace('.', '-')}.json"), text: "This is the most recent documentation, freshly created just for you!");

        await RespondOrFollowupAsync(text: "You've got a DM!", ephemeral: true);
    }
}