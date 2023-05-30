using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;

namespace BaseBotService.Utilities.Extensions;

public static class GuildConfigsExtensions
{
    public static string GetGuildSettingsName(this GuildConfigs configs, ITranslationService translationService)
    {
        string id = $"guildconfig-{configs.ToString().ToLowerKebabCase()}";

        // Add discord emoji based on the config
        return configs switch
        {
            GuildConfigs.Modroles => string.Concat(":shield:", " ", translationService.GetString(id)),
            GuildConfigs.Artistroles => string.Concat(":art:", " ", translationService.GetString(id)),
            _ => translationService.GetString(id),
        };
    }
}
