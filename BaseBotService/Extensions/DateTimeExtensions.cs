namespace BaseBotService.Extensions;
public static class DateTimeExtensions
{
    public static long ToUnixTimestamp(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    public static long ToUnixTimestamp(this DateTimeOffset dateTimeOffset) => (long)(dateTimeOffset - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;

    public static string ToDiscordTimestamp(this DateTime dateTime) => $"<t:{dateTime.ToUnixTimestamp()}:f>";

    public static string ToDiscordTimestamp(this DateTimeOffset dateTimeOffset) => $"<t:{dateTimeOffset.ToUnixTimestamp()}:f>";
}
