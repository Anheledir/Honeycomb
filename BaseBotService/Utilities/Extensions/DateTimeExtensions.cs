using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities.Enums;
using System.Globalization;

namespace BaseBotService.Utilities.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    public static long ToUnixTimestamp(this DateTimeOffset dateTimeOffset) => (long)(dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

    public static string ToDiscordTimestamp(this DateTime dateTime, ITranslationService translationService, DiscordTimestampFormat format = DiscordTimestampFormat.ShortDateTime)
        => dateTime.Equals(DateTime.MinValue)
        ? translationService.GetString("not-available")
        : $"<t:{dateTime.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";

    public static string ToDiscordTimestamp(this DateTimeOffset dateTimeOffset, ITranslationService translationService, DiscordTimestampFormat format = DiscordTimestampFormat.ShortDateTime)
        => dateTimeOffset.Equals(DateTimeOffset.MinValue)
        ? translationService.GetString("not-available")
        : $"<t:{dateTimeOffset.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";

    internal static char DiscordTimestampFormatHelper(this DiscordTimestampFormat format) => format switch
    {
        DiscordTimestampFormat.ShortTime => 't',
        DiscordTimestampFormat.LongTime => 'T',
        DiscordTimestampFormat.ShortDate => 'd',
        DiscordTimestampFormat.LongDate => 'D',
        DiscordTimestampFormat.ShortDateTime => 'f',
        DiscordTimestampFormat.LongDateTime => 'F',
        DiscordTimestampFormat.RelativeTime => 'R',
        _ => 'f'
    };

    public static DateTime ToTimezone(this DateTime dateTime, Timezone timezone)
    {
        int minutesToAdjust = (int)timezone;
        TimeSpan timeSpan = new(0, minutesToAdjust, 0);
        return dateTime + timeSpan;
    }

    public static DateTime ToUtcFromTimezone(this DateTime dateTime, Timezone timezone)
    {
        int minutesToAdjust = (int)timezone;
        TimeSpan timeSpan = new(0, -minutesToAdjust, 0);
        return dateTime + timeSpan;
    }

    public static int GetAge(this DateTime birthday)
    {
        DateTime today = DateTime.Today;
        int age = today.Year - birthday.Year;

        if (birthday > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public static string GetDayAndMonth(this DateTime date, Countries country)
    {
        DateTimeFormatInfo dtfi = country.GetCultureInfo().DateTimeFormat;
        string monthName = dtfi.GetMonthName(date.Month);
        string day = date.Day.ToString();
        return $"{day} {monthName}";
    }
}
