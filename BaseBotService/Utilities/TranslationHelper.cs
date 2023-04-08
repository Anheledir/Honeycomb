namespace BaseBotService.Utilities;

public static class TranslationHelper
{
    /// <summary>
    /// Creates a dictionary of arguments for use with translations that accept named arguments.
    /// </summary>
    /// <param name="name">The name of the first argument.</param>
    /// <param name="value">The value of the first argument.</param>
    /// <param name="args">A comma-separated list of additional name-value pairs, where each name is a string and each value is an object.</param>
    /// <returns>A dictionary containing the provided name-value pairs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either the name or the value is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the number of arguments is not a multiple of two, or when an argument name is an empty string.</exception>
    /// <example>
    /// <code>
    /// var args = TranslationHelper.Arguments("name", "John Doe", "age", 25);
    /// string message = translationService.GetString("greeting", args);
    /// </code>
    /// </example>
    public static Dictionary<string, object> Arguments(string? name, object value, params object[] args)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
        if (args.Length % 2 != 0)
        {
            throw new ArgumentException(
                "Expected a comma-separated list of name, value arguments, but the number of arguments is not a multiple of two", nameof(args));
        }

        Dictionary<string, object> argsDic = new()
        {
            { name, value }
        };

        for (int i = 0; i < args.Length; i += 2)
        {
            name = args[i] as string;
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(
                    $"Expected the argument at index {i} to be a non-empty string", nameof(args));
            }
            value = args[i + 1];
            if (value == null)
            {
                throw new ArgumentNullException(nameof(args),
                    $"Expected the argument at index {i + 1} to be a non-null value");
            }
            argsDic.Add(name, value);
        }

        return argsDic;
    }
}
