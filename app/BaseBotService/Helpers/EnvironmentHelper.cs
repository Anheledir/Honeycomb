using BaseBotService.Enumeration;
using BaseBotService.Exceptions;
using BaseBotService.Interfaces;
using Serilog;

namespace BaseBotService.Helpers
{
    public class EnvironmentHelper : IEnvironmentHelper
    {
        public EnvironmentHelper(ILogger logger)
        {
            string? token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
            if (string.IsNullOrWhiteSpace(token))
            {
                logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
                throw (new EnvironmentException(EnvironmentSettingEnum.DiscordBotToken, "Token was null or empty."));
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
                    RegisterCommands = RegisterCommandsOnStartupEnum.YesWithoutOverwrite;
                    break;
                case "2":
                    RegisterCommands = RegisterCommandsOnStartupEnum.YesWithOverwrite;
                    break;
                default:
                    RegisterCommands = RegisterCommandsOnStartupEnum.NoRegistration;
                    break;
            }
        }

        public string DiscordBotToken { get; }

        public RegisterCommandsOnStartupEnum RegisterCommands { get; }
    }
}