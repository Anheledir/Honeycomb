using BaseBotService.Enumeration;
using BaseBotService.Exceptions;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using Serilog;

namespace BaseBotService.Services;

public class EnvironmentService : IEnvironmentService
{
    private readonly ILogger _logger;

    public EnvironmentService(ILogger logger)
    {
        _logger = logger;

        string? token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
            throw new EnvironmentException(EnvironmentSetting.DiscordBotToken, "Token was null or empty.");
        }
        DiscordBotToken = token;
        _logger.Information($"Environment variable 'DISCORD_BOT_TOKEN' set to '{DiscordBotToken.MaskToken()}.'");

        string? cmdReg = Environment.GetEnvironmentVariable("COMMAND_REGISTER");
        if (string.IsNullOrWhiteSpace(cmdReg))
        {
            _logger.Warning("Environment variable 'COMMAND_REGISTER' not set, using default.");
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
        _logger.Information($"Environment variable 'COMMAND_REGISTER' set to '{(int)RegisterCommands}' ({RegisterCommands}).");

        string? healthPort = Environment.GetEnvironmentVariable("HEALTH_PORT");
        if (string.IsNullOrWhiteSpace(healthPort))
        {
            _logger.Warning("Environment variable 'HEALTH_PORT' not set, using default.");
        }
        else if (int.TryParse(healthPort, out int port))
        {
            _logger.Information($"Environment variable 'HEALTH_PORT' set to '{port}'.");
            HealthPort = port;
        }
        else
        {
            _logger.Warning("Environment variable 'HEALTH_PORT' has an invalid value, using default.");
        }

        string? dbPath = Environment.GetEnvironmentVariable("DATABASE_FILEPATH");
        if (string.IsNullOrWhiteSpace(dbPath))
        {
            _logger.Information($"Environment variable 'DATABASE_FILEPATH' not set, using default.");
        }
        else
        {
            _logger.Information($"Environment variable 'DATABASE_FILEPATH' set to '{dbPath}'.");
            DatabaseFile = dbPath;
        }

        if (UseAzureStorage)
        {
            _logger.Information($"Environment variable 'STORAGE_ACCOUNT_NAME' set to '{AzureStorageAccount}'.");
            _logger.Information($"Environment variable 'STORAGE_ACCOUNT_KEY' set to '{AzureStorageKey!.MaskToken()}'.");
            _logger.Information($"Environment variable 'STORAGE_ACCOUNT_CONTAINER' set to '{AzureStorageContainer}'.");
        }

        // logging the remaining values
        _logger.Information($"Environment variable 'ASPNETCORE_ENVIRONMENT' set to '{EnvironmentName}'.");
    }

    public string DiscordBotToken { get; }

    public RegisterCommandsOnStartup RegisterCommands { get; }

    public string EnvironmentName { get; } = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";

    public int HealthPort { get; } = 8080;

    public string DatabaseFile { get; } = "honeycomb.db";

    public bool UseAzureStorage => !string.IsNullOrWhiteSpace(AzureStorageKey) && !string.IsNullOrWhiteSpace(AzureStorageAccount) && !string.IsNullOrWhiteSpace(AzureStorageContainer);

    public string? AzureStorageAccount { get; } = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME");

    public string? AzureStorageKey { get; } = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_KEY");

    public string? AzureStorageContainer { get; } = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_CONTAINER");
}