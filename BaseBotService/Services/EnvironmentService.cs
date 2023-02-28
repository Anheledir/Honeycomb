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
    }

    public string DiscordBotToken { get; }

    public RegisterCommandsOnStartup RegisterCommands { get; }
}