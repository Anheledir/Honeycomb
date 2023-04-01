using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseBotService.Utilities.Extensions;

/// <summary>
/// Provides extension methods for string manipulation.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Repeats the input string a specified number of times.
    /// </summary>
    /// <param name="input">The input string to be repeated.</param>
    /// <param name="count">The number of times the input string should be repeated.</param>
    /// <returns>
    /// A new string consisting of the input string repeated the specified number of times.
    /// If the count parameter is less than or equal to 0, an empty string is returned.
    /// </returns>
    public static string Repeat(this string input, int count)
    {
        if (count <= 0)
        {
            return string.Empty;
        }

        return new StringBuilder(input.Length * count).Insert(0, input, count).ToString();
    }

    /// <summary>
    /// Masks a portion of the input token for security or display purposes.
    /// </summary>
    /// <param name="token">The input token to be masked.</param>
    /// <returns>
    /// A masked version of the input token, with the middle portion replaced by asterisks (*).
    /// If the input token is empty or null, an empty string is returned.
    /// </returns>
    /// <example>
    /// <code>
    /// string inputToken = "abcdefgh";
    /// string maskedToken = inputToken.MaskToken(); // Output: "abc**fgh"
    /// </code>
    /// </example>
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

    /// <summary>
    /// Extracts the leading emoji from the input string and returns the remaining string without the emoji.
    /// </summary>
    /// <param name="stringWithLeadingEmoji">The input string that may contain a leading emoji.</param>
    /// <param name="unicodeEmoji">When this method returns, contains the extracted Emoji object if an emoji was found at the start of the input string; otherwise, the default value for the Emoji type.</param>
    /// <returns>
    /// The input string without the leading emoji and optional whitespace. If no emoji was found at the start of the input string, the original string is returned.
    /// </returns>
    /// <remarks>
    /// This method uses a regex pattern to identify and extract the leading emoji in the input string. If an emoji is found, it is parsed and returned through the out parameter. The input string is then modified to remove the leading emoji and optional whitespace.
    /// </remarks>
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

    /// <summary>
    /// Converts a camel case string to a space-separated string.
    /// </summary>
    /// <param name="camelCase">The input camel case string.</param>
    /// <returns>
    /// A string with space-separated words derived from the input camel case string.
    /// </returns>
    /// <example>
    /// <code>
    /// string camelCaseString = "camelCaseExample";
    /// string spaceSeparatedString = camelCaseString.FromCamelCase(); // Output: "camel Case Example"
    /// </code>
    /// </example>
    public static string FromCamelCase(this string camelCase) => CamelCaseBoundaryRegex().Replace(camelCase, "$1 $2");

    /// <summary>
    /// Converts a camel case string to a kebab case string with all lowercase characters.
    /// </summary>
    /// <param name="camelCase">The input camel case string.</param>
    /// <returns>
    /// A string in kebab case with all lowercase characters, derived from the input camel case string.
    /// </returns>
    /// <example>
    /// <code>
    /// string camelCaseString = "camelCaseExample";
    /// string kebabCaseString = camelCaseString.ToLowerKebabCase(); // Output: "camel-case-example"
    /// </code>
    /// </example>
    public static string ToLowerKebabCase(this string camelCase) => camelCase.FromCamelCase().Replace(" ", "-").ToLowerInvariant();

    [GeneratedRegex("^:(\\w+):")]
    private static partial Regex EmojiCodeRegex();
    [GeneratedRegex("^:\\w+:\\s*")]
    private static partial Regex EmojiCodeWithOptionalWhitespaceRegex();
    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex CamelCaseBoundaryRegex();
}
