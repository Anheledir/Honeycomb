using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BaseBotService.Utilities.Extensions;
public static partial class StringExtensions
{
    public static string MaskToken(this string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return string.Empty;
        }

        int maskLength = Math.Max(token.Length / 3, 1);
        string mask = new('*', maskLength);

        return token.Length <= 2 * maskLength
            ? mask
            : string.Concat(token[..maskLength], mask, token[^maskLength..]);
    }

    public static string ExtractEmoji(this string stringWithLeadingEmoji, out Emoji unicodeEmoji)
    {
        unicodeEmoji = default!;

        // Parse the emoji using a regex
        Match match = EmojiCodeRegex().Match(stringWithLeadingEmoji);
        if (match.Success)
        {
            string emojiCode = match.Groups[1].Value;
            if (!Emoji.TryParse($":{emojiCode}:", out unicodeEmoji))
            {
                Debug.WriteLine(stringWithLeadingEmoji);
                Debug.WriteLine($"{emojiCode}");
            }
        }
        return EmojiCodeWithOptionalWhitespaceRegex().Replace(stringWithLeadingEmoji, string.Empty).TrimStart();
    }

    public static string FromCamelCase(this string camelCase) => CamelCaseBoundaryRegex().Replace(camelCase, "$1 $2");

    public static string ToLowerKebabCase(this string camelCase) => camelCase.FromCamelCase().Replace(" ", "-").ToLowerInvariant();

    [GeneratedRegex("^:(\\w+):")]
    private static partial Regex EmojiCodeRegex();
    [GeneratedRegex("^:\\w+:\\s*")]
    private static partial Regex EmojiCodeWithOptionalWhitespaceRegex();
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex CamelCaseBoundaryRegex();
}
