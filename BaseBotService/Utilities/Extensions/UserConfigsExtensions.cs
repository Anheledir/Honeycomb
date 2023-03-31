using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;

namespace BaseBotService.Utilities.Extensions;

public static class UserConfigsExtensions
{
    public static string GetUserSettingsName(this UserConfigs configs, ITranslationService translationService)
    {
        string id = $"userconfig-{configs.ToString().ToLowerKebabCase()}";

        // Add discord emoji based on the config
        return configs switch
        {
            UserConfigs.Country => ":earth_americas: " + translationService.GetString(id),
            UserConfigs.Languages => ":globe_with_meridians: " + translationService.GetString(id),
            UserConfigs.GenderIdentity => ":identification_card: " + translationService.GetString(id),
            UserConfigs.Timezone => ":watch: " + translationService.GetString(id),
            UserConfigs.Birthday => ":birthday: " + translationService.GetString(id),
            _ => translationService.GetString(id),
        };
    }
}
