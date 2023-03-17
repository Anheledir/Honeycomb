using BaseBotService.Enumeration;
using System.Text.RegularExpressions;

namespace BaseBotService.Extensions;

public static partial class TimezoneExtensions
{
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex PascalCasing();

    public static string ToNameWithOffset(this Timezone timezone)
    {
        string timezoneName = PascalCasing().Replace(timezone.ToString(), "$1 $2");
        int offsetHours = Math.Abs((int)timezone) / 60;
        int offsetMinutes = Math.Abs((int)timezone) % 60;
        string offsetString = $"{((int)timezone >= 0 ? "+" : "-")}{offsetHours:D2}:{offsetMinutes:D2}";
        return $"{timezoneName} ({offsetString})";
    }
}