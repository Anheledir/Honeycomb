namespace BaseBotService.Utilities.Enums;

/// <summary>
/// Enumeration representing different Discord time-stamp formats.
/// </summary>
public enum DiscordTimestampFormat
{
    /// <summary>
    /// Short time format (e.g., "16:20").
    /// </summary>
    /// <example>
    /// 16:20
    /// </example>
    ShortTime = 0,

    /// <summary>
    /// Long time format (e.g., "16:20:30").
    /// </summary>
    /// <example>
    /// 16:20:30
    /// </example>
    LongTime = 1,

    /// <summary>
    /// Short date format (e.g., "2023-03-14").
    /// </summary>
    /// <example>
    /// 2023-03-14
    /// </example>
    ShortDate = 2,

    /// <summary>
    /// Long date format (e.g., "Tuesday, March 14, 2023").
    /// </summary>
    /// <example>
    /// Tuesday, March 14, 2023
    /// </example>
    LongDate = 3,

    /// <summary>
    /// Short date-time format (e.g., "2023-03-14 16:20").
    /// </summary>
    /// <example>
    /// 2023-03-14 16:20
    /// </example>
    ShortDateTime = 4,

    /// <summary>
    /// Long date-time format (e.g., "Tuesday, March 14, 2023 16:20:30").
    /// </summary>
    /// <example>
    /// Tuesday, March 14, 2023 16:20:30
    /// </example>
    LongDateTime = 5,

    /// <summary>
    /// Relative time format (e.g., "2 hours ago").
    /// </summary>
    /// <example>
    /// 2 hours ago
    /// </example>
    RelativeTime = 6
}
