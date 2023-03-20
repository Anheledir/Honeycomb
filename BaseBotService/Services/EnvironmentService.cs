using BaseBotService.Enumeration;

namespace BaseBotService.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly DateTime _startupTime = DateTime.UtcNow;
    public EnvironmentService(ILogger logger)
    {
        DiscordBotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN")!;
        if (string.IsNullOrWhiteSpace(DiscordBotToken))
        {
            logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
            throw new ArgumentException("The Discord token must not be empty.");
        }
        logger.Information($"Environment variable 'DISCORD_BOT_TOKEN' set to '{DiscordBotToken.MaskToken()}.'");

        switch (Environment.GetEnvironmentVariable("COMMAND_REGISTER"))
        {
            case "1":
                RegisterCommands = RegisterCommandsOnStartup.YesWithoutOverwrite;
                break;
            case "2":
                RegisterCommands = RegisterCommandsOnStartup.YesWithOverwrite;
                break;
            default:
                RegisterCommands = RegisterCommandsOnStartup.NoRegistration;
                break;
        }
        logger.Information($"Mode for registering commands: '{(int)RegisterCommands}' ({RegisterCommands}).");

        HealthPort = int.Parse(Environment.GetEnvironmentVariable("HEALTH_PORT") ?? "8080");
        logger.Information($"http-port for health probe set to '{HealthPort}'.");

        DatabaseFile = Environment.GetEnvironmentVariable("DATABASE_FILEPATH") ?? "honeycomb.db";
        logger.Information($"Path and filename for our LiteDB database: '{DatabaseFile}'.");

        EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
        logger.Information($"Environment identifier is '{EnvironmentName}'.");
    }

    public string DiscordBotToken { get; }

    public RegisterCommandsOnStartup RegisterCommands { get; }

    public string EnvironmentName { get; }

    public int HealthPort { get; }

    public string DatabaseFile { get; }

    public string GetUptime()
    {
        TimeSpan uptime = DateTime.UtcNow - _startupTime;
        string daysString = uptime.Days == 1 ? "day" : "days";
        string hoursString = uptime.Hours == 1 ? "hour" : "hours";
        string minutesString = uptime.Minutes == 1 ? "minute" : "minutes";
        string secondsString = uptime.Seconds == 1 ? "second" : "seconds";
        return $"{uptime.Days} {daysString}, {uptime.Hours} {hoursString}, {uptime.Minutes} {minutesString}, and {uptime.Seconds} {secondsString}";
    }
}