using BaseBotService.Interactions.Enums;

namespace BaseBotService.Utilities.Extensions;

public static class UserConfigsExtensions
{
    public static string GetUserSettingsName(this UserConfigs configs)
    {
        string name = configs.ToString().FromCamelCase();

        // Add discord emoji based on the config
        return configs switch
        {
            UserConfigs.Country => ":earth_americas: " + name,
            UserConfigs.Languages => ":globe_with_meridians: " + name,
            UserConfigs.GenderIdentity => ":identification_card: " + name,
            UserConfigs.Timezone => ":watch: " + name,
            UserConfigs.Birthday => ":birthday: " + name,
            UserConfigs.Pronouns => ":speaking_head: " + name,
            UserConfigs.SocialStyle => ":handshake: " + name,
            UserConfigs.RelationshipStatus => ":couple: " + name,
            _ => name,
        };
    }
}
