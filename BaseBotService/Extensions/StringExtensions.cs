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
}
