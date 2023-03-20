using BaseBotService.Enumeration;

namespace BaseBotService.Extensions;

public static partial class UserConfigsExtensions
{
    public static string GetUserSettingsName(this UserConfigs configs)
    {
        string name = configs.ToString().FromCamelCase();

        // Add discord emoji based on the config
        switch (configs)
        {
            case UserConfigs.Country:
                name = ":earth_americas: " + name;
                break;
            case UserConfigs.Languages:
                name = ":globe_with_meridians: " + name;
                break;
            case UserConfigs.GenderIdentity:
                name = ":identification_card: " + name;
                break;
            case UserConfigs.Timezone:
                name = ":watch: " + name;
                break;
            case UserConfigs.Birthday:
                name = ":birthday: " + name;
                break;
            case UserConfigs.Pronouns:
                name = ":speaking_head: " + name;
                break;
            case UserConfigs.SocialStyle:
                name = ":handshake: " + name;
                break;
            case UserConfigs.RelationshipStatus:
                name = ":couple: " + name;
                break;
        }
        return name;
    }
}
