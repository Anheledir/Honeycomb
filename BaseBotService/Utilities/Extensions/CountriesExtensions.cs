using BaseBotService.Commands.Enums;
using System.Text.RegularExpressions;

namespace BaseBotService.Utilities.Extensions;

public static partial class CountriesExtensions
{
    private static readonly Dictionary<Countries, string> _countryFlagEmojis = new()
    {
        {Countries.UnitedStates, ":flag_us:"},
        {Countries.Canada, ":flag_ca:"},
        {Countries.UnitedKingdom, ":flag_gb:"},
        {Countries.Australia, ":flag_au:"},
        {Countries.Germany, ":flag_de:"},
        {Countries.France, ":flag_fr:"},
        {Countries.Brazil, ":flag_br:"},
        {Countries.Mexico, ":flag_mx:"},
        {Countries.Netherlands, ":flag_nl:"},
        {Countries.Sweden, ":flag_se:"},
        {Countries.Norway, ":flag_no:"},
        {Countries.Denmark, ":flag_dk:"},
        {Countries.Finland, ":flag_fi:"},
        {Countries.Spain, ":flag_es:"},
        {Countries.Italy, ":flag_it:"},
        {Countries.Poland, ":flag_pl:"},
        {Countries.Japan, ":flag_jp:"},
        {Countries.SouthKorea, ":flag_kr:"},
        {Countries.China, ":flag_cn:"},
        {Countries.Turkey, ":flag_tr:"},
        {Countries.Indonesia, ":flag_id:"},
        {Countries.Philippines, ":flag_ph:"},
        {Countries.Austria, ":flag_at:"},
        {Countries.Swiss, ":flag_ch:"},
    };

    public static string GetCountryNameWithFlag(this Countries country)
    {
        if (!_countryFlagEmojis.TryGetValue(country, out string? emoji))
        {
            return country.ToString();
        }

        string countryName = PascalCasing().Replace(country.ToString(), "$1 $2");
        return $"{emoji} {countryName}";
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex PascalCasing();
}
