using BaseBotService.Interactions.Enums;

namespace BaseBotService.Utilities.Extensions;

public static class GenderIdentityExtensions
{
    private static readonly Dictionary<GenderIdentity, string> _genderFlagEmojis = new()
    {
        { GenderIdentity.Unknown, ":star:" },
        { GenderIdentity.Male, ":male_sign:" },
        { GenderIdentity.Female, ":female_sign:" },
        { GenderIdentity.NonBinary, ":transgender_symbol:" },
        { GenderIdentity.TransgenderMale, ":transgender_flag:" },
        { GenderIdentity.TransgenderFemale, ":transgender_flag:" },
        { GenderIdentity.Genderqueer, ":transgender_symbol:" },
        { GenderIdentity.Other, ":grey_question:" },
    };

    public static string GetFlaggedGenderName(this GenderIdentity genderIdentity)
    {
        string genderName = genderIdentity.ToString();
        string? emoji = _genderFlagEmojis.GetValueOrDefault(genderIdentity);
        if (!string.IsNullOrWhiteSpace(emoji))
        {
            genderName = $"{emoji} {genderName}";
        }
        return genderName;
    }
}
