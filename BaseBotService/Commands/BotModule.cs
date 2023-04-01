using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Attributes;

namespace BaseBotService.Commands;

[Group("bot", "The main bot information module of Honeycomb.")]
[EnabledInDm(true)]
public class BotModule : BaseModule
{
    private readonly ITranslationService _translationService;

    public BotModule(ILogger logger, ITranslationService translationService)
    {
        Logger = logger.ForContext<BotModule>();
        _translationService = translationService;
    }

    [SlashCommand("about", "Returns information like runtime and current version of this Honeycomb bot instance.")]
    public async Task AboutAsync()
    {
        // Create embedded message with bot information
        EmbedBuilder response = GetEmbedBuilder()
            .WithTitle(AssemblyService.Name)
            .WithThumbnailUrl(BotUser.GetAvatarUrl())
            .WithUrl(_translationService.GetString("bot-website"))
            .WithDescription(_translationService.GetString("bot-description"))
            .WithFields(
                new EmbedFieldBuilder()
                .WithName(_translationService.GetString("uptime"))
                .WithValue(EnvironmentService.GetUptime()),
                new EmbedFieldBuilder()
                .WithName(_translationService.GetString("total-servers"))
                .WithValue(Context.Client.Guilds.Count)
                .WithIsInline(true)
            )
            .WithColor(Color.DarkPurple);
        await RespondOrFollowupAsync(embed: response.Build());
    }

    [SlashCommand("ping", "Pings the bot and returns its latency.")]
    public async Task PingAsync()
        => await RespondOrFollowupAsync(text: _translationService.GetString("ping-response", TranslationHelper.Arguments("latency", Context.Client.Latency)), ephemeral: true);

    [SlashCommand("documentation", "Sends a json-file via DM containing all command documentations.")]
    [RateLimit(1, 300)]
    public async Task DocumentationAsync()
    {
        if (!await CheckRateLimitAsync())
        {
            await RespondOrFollowupAsync(text: _translationService.GetString("error-rate-limit"));
            return;
        }
        await DeferAsync();

        string jsonString = Utilities.DocumentationUtility.GenerateDocumentationJson();
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
        using MemoryStream stream = new(jsonBytes);

        IDMChannel dm = await Caller.CreateDMChannelAsync();
        await dm.SendFileAsync(new FileAttachment(
            stream,
            _translationService.GetString(
                "documentation-filename",
                TranslationHelper.Arguments(
                    "version",
                    AssemblyService.Version.Replace('.', '-')
                )
            )
        ),
        text: _translationService.GetString("documentation-created"));

        await RespondOrFollowupAsync(text: _translationService.GetString("follow-up-in-DM"), ephemeral: true);
    }
}
