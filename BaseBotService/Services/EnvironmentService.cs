using BaseBotService.Enumeration;
using BaseBotService.Exceptions;
using BaseBotService.Interfaces;
using Serilog;

namespace BaseBotService.Services;

public class EnvironmentService : IEnvironmentService
{
    public EnvironmentService(ILogger logger)
    {
        string? token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
            throw new EnvironmentException(EnvironmentSetting.DiscordBotToken, "Token was null or empty.");
        }
        DiscordBotToken = token;

        string? cmdReg = Environment.GetEnvironmentVariable("COMMAND_REGISTER");
        if (string.IsNullOrWhiteSpace(cmdReg))
        {
            logger.Warning("Environment variable 'COMMAND_REGISTER' not set, using default.");
        }
        switch (cmdReg)
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
        logger.Information($"Environment variable 'COMMAND_REGISTER' set to '{(int)RegisterCommands}' ({RegisterCommands}).");

        logger.Information($"Environment variable 'ASPNETCORE_ENVIRONMENT' set to '{EnvironmentName}'.");

        string? healthPort = Environment.GetEnvironmentVariable("HEALTH_PORT");
        if (string.IsNullOrWhiteSpace(healthPort))
        {
            logger.Warning("Environment variable 'HEALTH_PORT' not set, using default.");
        }
        else if (int.TryParse(healthPort, out int port))
        {
            logger.Information($"Environment variable 'HEALTH_PORT' set to '{port}'.");
            HealthPort = port;
        }
        else
        {
            logger.Warning("Environment variable 'HEALTH_PORT' has an invalid value, using default.");

        }
    }

    public string DiscordBotToken { get; }

    public RegisterCommandsOnStartup RegisterCommands { get; }

    public string EnvironmentName { get; } = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";

    public int HealthPort { get; } = 8080;
}