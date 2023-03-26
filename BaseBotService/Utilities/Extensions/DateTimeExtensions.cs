using BaseBotService.Commands.Enums;
using BaseBotService.Utilities.Enums;

namespace BaseBotService.Utilities.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    public static long ToUnixTimestamp(this DateTimeOffset dateTimeOffset) => (long)(dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

    public static string ToDiscordTimestamp(this DateTime dateTime, DiscordTimestampFormat format = DiscordTimestampFormat.ShortDateTime)
        => dateTime.Equals(DateTime.MinValue)
        ? "n/a"
        : $"<t:{dateTime.ToUnixTimestamp()}:{format.DiscordTimestampFormatHelper()}>";

    public static string ToDiscordTimestamp(this DateTimeOffset dateTimeOffset, DiscordTimestampFormat format = DiscordTimestampFormat.ShortDateTime)
        => dateTimeOffset.Equals(DateTimeOffset.MinValue)
        ? "n/a"
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
}
