using BaseBotService.Core.Interfaces;
using Fluent.Net;
using System.Globalization;

/// <summary>
/// Provides methods for translating text based on the Fluent localization system.
/// </summary>
namespace BaseBotService.Infrastructure.Services;

public class TranslationService : ITranslationService
{
    private readonly IEnumerable<MessageContext> _contexts = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationService"/> class.
    /// </summary>
    /// <param name="contexts">A collection of MessageContext instances.</param>
    /// <exception cref="ArgumentNullException">Thrown when contexts is null.</exception>
    public TranslationService(IEnumerable<MessageContext> contexts)
    {
        _contexts = contexts ?? throw new ArgumentNullException(nameof(contexts));
    }

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
    /// var args = TranslationService.Arguments("name", "John Doe", "age", 25);
    /// string message = translationService.GetString("greeting", args);
    /// </code>
    /// </example>
    public Dictionary<string, object> Arguments(string? name, object value, params object[] args)
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
            throw new ArgumentException("Expected a comma-separated list of name, value arguments, but the number of arguments is not a multiple of two", nameof(args));
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
                throw new ArgumentException($"Expected the argument at index {i} to be a non-empty string", nameof(args));
            }
            value = args[i + 1];
            if (value == null)
            {
                throw new ArgumentNullException(nameof(args), $"Expected the argument at index {i + 1} to be a non-null value");
            }
            argsDic.Add(name, value);
        }

        return argsDic;
    }

    /// <summary>
    /// Retrieves the translated string for a given message ID and locale.
    /// </summary>
    /// <param name="id">The ID of the message to translate.</param>
    /// <param name="locale">The desired locale for the translated message.</param>
    /// <param name="args">An optional dictionary of arguments to pass to the translation. Default is null.</param>
    /// <param name="errors">An optional collection of FluentError instances to collect errors during translation. Default is null.</param>
    /// <returns>The translated message in the specified locale, or an empty string if the message ID or locale is not found.</returns>
    /// <example>
    /// <code>
    /// var translationService = new TranslationService(/* message contexts */);
    /// string messageId = "example-message";
    /// string translatedTextInEnglish = translationService.GetString(messageId, "en");
    /// string translatedTextInGerman = translationService.GetString(messageId, "de");
    /// string translatedTextInSpanish = translationService.GetString(messageId, "es");
    /// </code>
    /// </example>
    public string GetString(string id, string locale, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null)
    {
        var context = _contexts.FirstOrDefault(c => c.Locales.Contains(locale, StringComparer.InvariantCultureIgnoreCase));
        if (context != null)
        {
            Fluent.Net.RuntimeAst.Message msg = context.GetMessage(id);
            if (msg != null)
            {
                return context.Format(msg, args, errors);
            }
        }

        // If translation is not found in the requested locale, try English as a fall-back
        return locale != "en"
            ? GetString(id, "en", args, errors)
            : string.Empty;
    }

    /// <summary>
    /// Retrieves the translated string for a given message ID using the preferred locale.
    /// </summary>
    /// <param name="id">The ID of the message to translate.</param>
    /// <param name="args">An optional dictionary of arguments to pass to the translation. Default is null.</param>
    /// <param name="errors">An optional collection of FluentError instances to collect errors during translation. Default is null.</param>
    /// <returns>The translated message in the preferred locale, or an English fall-back translation if the message ID is not found in the preferred locale. Returns an empty string if the message ID is not found in any locale.</returns>
    /// <example>
    /// <code>
    /// var translationService = new TranslationService(/* message contexts */);
    /// string messageId = "example-message";
    /// string translatedText = translationService.GetString(messageId);
    /// </code>
    /// </example>
    public string GetString(string id, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null) => GetString(id, PreferredLocale, args, errors);

    /// <summary>
    /// Gets the preferred locale used by the translation service.
    /// </summary>
    public string PreferredLocale => _contexts.First().Locales.First();

    private CultureInfo _culture = null!;

    /// <summary>
    /// Gets the culture associated with the preferred locale used by the translation service.
    /// </summary>
    public CultureInfo Culture
    {
        get
        {
            _culture ??= new CultureInfo(PreferredLocale);
            return _culture;
        }
    }
}