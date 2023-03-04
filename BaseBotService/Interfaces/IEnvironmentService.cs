using BaseBotService.Enumeration;

namespace BaseBotService.Interfaces
{
    public interface IEnvironmentService
    {
        string DiscordBotToken { get; }
        RegisterCommandsOnStartup RegisterCommands { get; }
        string EnvironmentName { get; }
        int HealthPort { get; }
        string DatabaseFile { get; }
        bool UseAzureStorage { get; }
        string? AzureStorageAccount { get; }
        string? AzureStorageKey { get; }
        string? AzureStorageContainer { get; }
    }
}