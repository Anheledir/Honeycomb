using BaseBotService.Exceptions;
using BaseBotService.Interfaces;
using Serilog;

namespace BaseBotService.Helpers
{
    public class EnvironmentHelper : IEnvironmentHelper
    {
        public EnvironmentHelper(ILogger logger)
        {
            // Read environmental variables to initialize configuration
            string? token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
            if (string.IsNullOrWhiteSpace(token))
            {
                logger.Fatal("Environment variable 'DISCORD_BOT_TOKEN' not set.");
                throw (new EnvironmentException(EnvironmentSettingEnum.DiscordBotToken, "Token was null or empty."));
            }
            DiscordBotToken = token;
        }

        public string DiscordBotToken { get; }
    }
}