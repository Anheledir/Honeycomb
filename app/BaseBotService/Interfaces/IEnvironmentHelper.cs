using BaseBotService.Enumeration;

namespace BaseBotService.Interfaces
{
    public interface IEnvironmentHelper
    {
        string DiscordBotToken { get; }
        RegisterCommandsOnStartupEnum RegisterCommands { get; }
    }
}