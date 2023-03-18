using System.Diagnostics;
using System.Text.RegularExpressions;
using Emoji = Discord.Emoji;

namespace BaseBotService.Extensions;
public static class StringExtensions
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
            : string.Concat(token.Substring(0, maskLength), mask, token.Substring(token.Length - maskLength));
    }

    public static string ExtractEmoji(this string stringWithLeadingEmoji, out Emoji unicodeEmoji)
    {
        unicodeEmoji = default!;

        // Parse the emoji using a regex
        Match match = Regex.Match(stringWithLeadingEmoji, "^:(\\w+):");
        if (match.Success)
        {
            string emojiCode = match.Groups[1].Value;
            if (!Emoji.TryParse($":{emojiCode}:", out unicodeEmoji))
            {
                Debug.WriteLine(stringWithLeadingEmoji);
                Debug.WriteLine($"{emojiCode}");
            }
        }
        return Regex.Replace(stringWithLeadingEmoji, "^:\\w+:\\s*", string.Empty).TrimStart();
    }
}
