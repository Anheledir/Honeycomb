using BaseBotService.Enumeration;

namespace BaseBotService.Extensions;


public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    public static long ToUnixTimestamp(this DateTimeOffset dateTimeOffset) => (long)(dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

    public static string ToDiscordTimestamp(this DateTime dateTime, DiscordTimestampFormat format = DiscordTimestampFormat.ShortDateTime)
        => dateTime.Equals(DateTime.MinValue)
        ? "n/a"
        : $"<t:{dateTime.ToUnixTimestamp()}:{DiscordTimestampFormatHelper(format)}>";

    public static string ToDiscordTimestamp(this DateTimeOffset dateTimeOffset, DiscordTimestampFormat format = DiscordTimestampFormat.ShortDateTime)
        => dateTimeOffset.Equals(DateTimeOffset.MinValue)
        ? "n/a"
        : $"<t:{dateTimeOffset.ToUnixTimestamp()}:{DiscordTimestampFormatHelper(format)}>";

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
}
