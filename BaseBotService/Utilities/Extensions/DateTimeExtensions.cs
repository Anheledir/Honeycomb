using BaseBotService.Commands.Enums;
using BaseBotService.Core.Interfaces;
using BaseBotService.Utilities.Enums;
using System.Globalization;

namespace BaseBotService.Utilities.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a DateTime to a Unix timestamp.
    /// </summary>
    /// <param name="dateTime">The DateTime to be converted.</param>
    /// <returns>A long representing the Unix timestamp.</returns>
    /// <example>
    /// long unixTimestamp = dateTime.ToUnixTimestamp();
    /// </example>
    public static long ToUnixTimestamp(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    /// <summary>
    /// Converts a DateTimeOffset to a Unix timestamp.
    /// </summary>
    /// <param name="dateTimeOffset">The DateTimeOffset to be converted.</param>
    /// <returns>A long representing the Unix timestamp.</returns>
    /// <example>
    /// long unixTimestamp = dateTimeOffset.ToUnixTimestamp();
    /// </example>
    public static long ToUnixTimestamp(this DateTimeOffset dateTimeOffset) => (long)(dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

    /// <summary>
    /// Converts a DateTime to a Discord-formatted timestamp string.
    /// </summary>
    /// <param name="dateTime">The DateTime to be converted.</param>
    /// <param name="translationService">The ITranslationService instance.</param>
    /// <param name="format">The DiscordTimestampFormat to use. Defaults to ShortDateTime.</param>
    /// <returns>A string representing the Discord-formatted timestamp.</returns>
    /// <example>
    /// string discordTimestamp = dateTime.ToDiscordTimestamp(translationService);
    /// </example>
    public static string ToDiscordTimestamp(this DateTime dateTime, ITranslationService translationService, DiscordTimestampFormat format = DiscordTimestampFormat.ShortDateTime)
        => dateTime.Equals(DateTime.MinValue)
        ? translationService.GetString("not-available")
        : $"<t:{dateTime.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";

    /// <summary>
    /// Converts a DateTimeOffset to a Discord-formatted timestamp string.
    /// </summary>
    /// <param name="dateTimeOffset">The DateTimeOffset to be converted.</param>
    /// <param name="translationService">The ITranslationService instance.</param>
    /// <param name="format">The DiscordTimestampFormat to use. Defaults to ShortDateTime.</param>
    /// <returns>A string representing the Discord-formatted timestamp.</returns>
    /// <example>
    /// string discordTimestamp = dateTimeOffset.ToDiscordTimestamp(translationService);
    /// </example>
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

    /// <summary>
    /// Calculates the age based on a given birthdate.
    /// </summary>
    /// <param name="birthday">The birthdate.</param>
    /// <returns>An integer representing the age.</returns>
    /// <example>
    /// int age = birthday.GetAge();
    /// </example>
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

    /// <summary>
    /// Gets the day and month of a DateTime in the format of the specified country.
    /// </summary>
    /// <param name="date">The DateTime to be formatted.</param>
    /// <param name="country">The Countries enumeration value representing the country.</param>
    /// <returns>A string representing the day and month in the format of the specified country.</returns>
    /// <example>
    /// string dayAndMonth = date.GetDayAndMonth(Countries.USA);
    /// </example>
    public static string GetDayAndMonth(this DateTime date, Countries country)
    {
        DateTimeFormatInfo dtfi = country.GetCultureInfo().DateTimeFormat;
        string monthName = dtfi.GetMonthName(date.Month);
        string day = date.Day.ToString();
        return $"{day} {monthName}";
    }

    /// <summary>
    /// Converts a DateTime to an Instant.
    /// </summary>
    /// <param name="dateTime">The DateTime to be converted.</param>
    /// <returns>An Instant representing the same point in time as the input DateTime.</returns>
    /// <example>
    /// var instant = dateTime.ToDateTimeInstant();
    /// </example>
    public static Instant ToInstant(this DateTime dateTime)
    {
        return Instant.FromDateTimeUtc(dateTime.ToUniversalTime());
    }

    /// <summary>
    /// Converts a LocalDate to an Instant.
    /// </summary>
    /// <param name="localDate">The LocalDate to be converted.</param>
    /// <param name="timeZone">The time zone used for the conversion.</param>
    /// <returns>An Instant representing the start of the LocalDate in the specified time zone.</returns>
    /// <example>
    /// var instant = localDate.ToLocalDateInstant(DateTimeZone.Utc);
    /// </example>
    public static Instant ToInstant(this LocalDate localDate, DateTimeZone timeZone)
    {
        LocalDateTime localDateTime = localDate.AtMidnight();
        return localDateTime.InZoneLeniently(timeZone).ToInstant();
    }

    /// <summary>
    /// Converts a DateTime to a LocalDate.
    /// </summary>
    /// <param name="dateTime">The DateTime to be converted.</param>
    /// <returns>A LocalDate representing the same date as the input DateTime.</returns>
    /// <example>
    /// var localDate = dateTime.ToDateTimeLocalDate();
    /// </example>
    public static LocalDate ToLocalDate(this DateTime dateTime)
    {
        return LocalDate.FromDateTime(dateTime.Date);
    }

    /// <summary>
    /// Converts an Instant to a LocalDate.
    /// </summary>
    /// <param name="instant">The Instant to be converted.</param>
    /// <param name="timeZone">The time zone used for the conversion.</param>
    /// <returns>A LocalDate representing the date of the Instant in the specified time zone.</returns>
    /// <example>
    /// var localDate = instant.ToInstantLocalDate(DateTimeZone.Utc);
    /// </example>
    public static LocalDate ToLocalDate(this Instant instant, DateTimeZone timeZone)
    {
        return instant.InZone(timeZone).LocalDateTime.Date;
    }
}
