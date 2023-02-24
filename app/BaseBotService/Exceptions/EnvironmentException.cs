namespace BaseBotService.Exceptions
{
    public enum EnvironmentSettingEnum
    {
        Unknown = 0,
        DiscordBotToken = 1
    }

    public class EnvironmentException : Exception
    {
        public EnvironmentSettingEnum Setting { get; set; }

        public EnvironmentException(EnvironmentSettingEnum setting, string message) : base(message)
        {
            Setting = setting;
        }
    }
}
