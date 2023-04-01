using BaseBotService.Core.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Extensions;

namespace BaseBotService.Infrastructure.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly DateTime _startupTime = DateTime.UtcNow;
    private readonly ITranslationService _translationService;

    public EnvironmentService(ILogger logger, ITranslationService translationService)
    {
        DiscordBotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN")!;
        if (string.IsNullOrWhiteSpace(DiscordBotToken))
        {
            logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
            throw new ArgumentException("The Discord token must not be empty.");
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

        DatabaseFile = Environment.GetEnvironmentVariable("DATABASE_FILEPATH") ?? "honeycomb.db";
        logger.Information($"Path and filename for our LiteDB database: '{DatabaseFile}'.");

        EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
        logger.Information($"Environment identifier is '{EnvironmentName}'.");
        _translationService = translationService;
    }

    public string DiscordBotToken { get; }

    public RegisterCommandsOnStartup RegisterCommands { get; }

    public string EnvironmentName { get; }

    public int HealthPort { get; }

    public string DatabaseFile { get; }

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