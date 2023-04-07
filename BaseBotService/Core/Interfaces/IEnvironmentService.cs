using BaseBotService.Core.Enums;

namespace BaseBotService.Core.Interfaces;

public interface IEnvironmentService
{
    string DiscordBotToken { get; }
    RegisterCommandsOnStartup RegisterCommands { get; }
    string EnvironmentName { get; }
    int HealthPort { get; }
    string ConnectionString { get; }
    string GetUptime();
}