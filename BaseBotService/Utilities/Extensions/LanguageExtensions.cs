using BaseBotService.Commands.Enums;
using System.Globalization;

namespace BaseBotService.Utilities.Extensions;

public static class CountryLanguageExtensions
{
    public static string GetFlaggedLanguageName(this Languages language)
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

        languageCode = $":flag_{languageCode}:";
        string languageName = language.ToString();

        // Capitalize first letter of each word
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        languageName = textInfo.ToTitleCase(languageName.ToLower().Replace("_", " "));

        return $"{languageCode} {languageName}";
    }
}