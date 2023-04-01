using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;

namespace BaseBotService.Utilities.Extensions;

public static class GenderIdentityExtensions
{
    private static readonly Dictionary<GenderIdentity, string> _genderFlagEmojis = new()
    {
        { GenderIdentity.Unknown, ":star:" },
        { GenderIdentity.Male, ":male_sign:" },
        { GenderIdentity.Female, ":female_sign:" },
        { GenderIdentity.NonBinary, ":transgender_flag:" },
        { GenderIdentity.TransgenderMale, ":transgender_flag:" },
        { GenderIdentity.TransgenderFemale, ":transgender_flag:" },
        { GenderIdentity.Genderqueer, ":transgender_flag:" },
        { GenderIdentity.Other, ":grey_question:" },
    };

    public static string GetFlaggedGenderName(this GenderIdentity genderIdentity, ITranslationService translationService)
    {
        string id = $"gender-{genderIdentity.ToString().ToLowerKebabCase()}";
        string genderName = translationService.GetString(id);
        string? emoji = _genderFlagEmojis.GetValueOrDefault(genderIdentity);
        if (!string.IsNullOrWhiteSpace(emoji))
        {
            genderName = $"{emoji} {genderName}";
        }
        return genderName;
    }
}
