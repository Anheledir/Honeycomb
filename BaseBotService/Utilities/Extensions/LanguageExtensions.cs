﻿using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;

namespace BaseBotService.Utilities.Extensions;

public static class CountryLanguageExtensions
{
    public static string GetFlaggedLanguageNames(this Languages languages, ITranslationService translationService)
    {
        var languageNames = new List<string>();

        foreach (Languages language in Enum.GetValues(typeof(Languages)))
        {
            int flagValue = Convert.ToInt32(language);
            if (flagValue != 0 && (Convert.ToInt32(languages) & flagValue) == flagValue)
            {
                string languageName = GetFlaggedLanguageName(language, translationService);
                languageNames.Add(languageName);
            }
        }

        return languageNames.Any() ? string.Join("\n", languageNames) : Languages.Other.GetFlaggedLanguageName(translationService);
    }

    private static string GetFlaggedLanguageName(this Languages language, ITranslationService translationService)
    {
        string languageCode;
        // Convert language code to flag emoji
        switch (language)
        {
            case Languages.English:
                languageCode = "gb";
                break;
            case Languages.German:
                languageCode = "de";
                break;
            case Languages.French:
                languageCode = "fr";
                break;
            case Languages.Portuguese:
                languageCode = "pt";
                break;
            case Languages.Spanish:
                languageCode = "es";
                break;
            case Languages.Dutch:
                languageCode = "nl";
                break;
            case Languages.Swedish:
                languageCode = "se";
                break;
            case Languages.Norwegian:
                languageCode = "no";
                break;
            case Languages.Danish:
                languageCode = "dk";
                break;
            case Languages.Finnish:
                languageCode = "fi";
                break;
            case Languages.Polish:
                languageCode = "pl";
                break;
            case Languages.Russian:
                languageCode = "ru";
                break;
            case Languages.Japanese:
                languageCode = "jp";
                break;
            case Languages.Korean:
                languageCode = "kr";
                break;
            case Languages.Chinese:
                languageCode = "cn";
                break;
            case Languages.Hindi:
                languageCode = "in";
                break;
            case Languages.Turkish:
                languageCode = "tr";
                break;
            case Languages.Indonesian:
                languageCode = "id";
                break;
            case Languages.Filipino_Tagalog:
                languageCode = "ph";
                break;
            case Languages.Italian:
                languageCode = "it";
                break;
            case Languages.Arabic:
                languageCode = "sa";
                break;
            case Languages.Thai:
                languageCode = "th";
                break;
            case Languages.Vietnamese:
                languageCode = "vn";
                break;
            case Languages.Greek:
                languageCode = "gr";
                break;
            default:
                return language.ToString();
        }

        string languageName = language.ToString();
        string localizedLanguageName = translationService.GetString($"language-{languageName.ToLowerKebabCase()}");

        return $":flag_{languageCode}: {localizedLanguageName}";
    }
}
