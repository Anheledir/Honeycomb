using BaseBotService.Enumeration;

namespace BaseBotService.Interfaces
{
    public interface IEnvironmentService
    {
        string DiscordBotToken { get; }
        RegisterCommandsOnStartup RegisterCommands { get; }
        string EnvironmentName { get; }
    }
}