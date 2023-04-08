using BaseBotService.Core.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Extensions;

namespace BaseBotService.Infrastructure.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly DateTime _startupTime = DateTime.UtcNow;
    private readonly ITranslationService _translationService;

    public EnvironmentService(ILogger logger, ITranslationService translationService, CancellationTokenSource cts)
    {
        DiscordBotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN")!;
        if (string.IsNullOrWhiteSpace(DiscordBotToken))
        {
            logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
            cts.Cancel();
        }
        logger.Information($"Environment variable 'DISCORD_BOT_TOKEN' set to '{DiscordBotToken.MaskToken()}.'");

        RegisterCommands = Environment.GetEnvironmentVariable("COMMAND_REGISTER") switch
        {
            "1" => RegisterCommandsOnStartup.YesWithoutOverwrite,
            "2" => RegisterCommandsOnStartup.YesWithOverwrite,
            _ => RegisterCommandsOnStartup.NoRegistration,
        };
        logger.Information($"Mode for registering commands: '{(int)RegisterCommands}' ({RegisterCommands}).");

        HealthPort = int.Parse(Environment.GetEnvironmentVariable("HEALTH_PORT") ?? "8080");
        logger.Information($"http-port for health probe set to '{HealthPort}'.");

        ConnectionString = Environment.GetEnvironmentVariable("ConnectionString") ?? "Filename=honeycomb.db;";
        logger.Information($"LiteDB database connection string: '{ConnectionString}'.");

        EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
        logger.Information($"Environment identifier is '{EnvironmentName}'.");
        _translationService = translationService;
    }

    public string DiscordBotToken { get; }

    public RegisterCommandsOnStartup RegisterCommands { get; }

    public string EnvironmentName { get; }

    public int HealthPort { get; }

    public string ConnectionString { get; }

    public string GetUptime()
    {
        TimeSpan uptime = DateTime.UtcNow - _startupTime;
        string result = _translationService.GetString("uptime-format", TranslationHelper.Arguments(
            "days", uptime.Days,
            "hours", uptime.Hours,
            "minutes", uptime.Minutes,
            "seconds", uptime.Seconds));
        return result;
    }
}