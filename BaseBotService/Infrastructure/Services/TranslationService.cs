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
    /// Retrieves the translated string for a given message ID and locale.
    /// </summary>
    /// <param name="id">The ID of the message to translate.</param>
    /// <param name="locale">The desired locale for the translated message.</param>
    /// <param name="args">An optional dictionary of arguments to pass to the translation. Default is null.</param>
    /// <param name="errors">An optional collection of FluentError instances to collect errors during translation. Default is null.</param>
    /// <returns>The translated message in the specified locale, or an empty string if the message ID or locale is not found.</returns>
    /// <exception cref="ArgumentException">Thrown if 'id' or 'locale' is null or empty.</exception>
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
        => GetString(id, null, locale, args, errors);

    /// <summary>
    /// Gets the translated string for a given ID and, optionally, an attribute.
    /// </summary>
    /// <param name="id">The ID of the translation entry.</param>
    /// <param name="attribute">The attribute of the translation entry (optional).</param>
    /// <param name="locale">The desired locale for the translation.</param>
    /// <param name="args">The arguments to be passed to the translation (optional).</param>
    /// <param name="errors">A collection of errors that occurred during the translation (optional).</param>
    /// <returns>The translated string for the given ID and attribute in the specified locale.</returns>
    /// <exception cref="ArgumentException">Thrown if 'id' or 'locale' is null or empty.</exception>
    public string GetString(string id, string? attribute, string locale, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
        }

        if (string.IsNullOrEmpty(locale))
        {
            throw new ArgumentException($"'{nameof(locale)}' cannot be null or empty.", nameof(locale));
        }

        var context = _contexts.FirstOrDefault(c => c.Locales.Contains(locale, StringComparer.InvariantCultureIgnoreCase));
        if (context != null)
        {
            Fluent.Net.RuntimeAst.Message msg = context.GetMessage(id);
            if (msg != null)
            {
                // Use the attribute if provided and found within the message
                Fluent.Net.RuntimeAst.Node? node = null;
                if (!string.IsNullOrEmpty(attribute))
                {
                    msg.Attributes.TryGetValue(attribute, out node);
                }
                node ??= msg.Value;

                if (node != null)
                {
                    return context.Format(node, args, errors);
                }
            }
        }

        // If translation is not found in the requested locale, try English as a fall-back
        return locale != "en"
            ? GetString(id, attribute, "en", args, errors)
            : string.Empty;
    }

    /// <summary>
    /// Retrieves the translated string for a given message ID using the preferred locale.
    /// </summary>
    /// <param name="id">The ID of the message to translate.</param>
    /// <param name="args">An optional dictionary of arguments to pass to the translation. Default is null.</param>
    /// <param name="errors">An optional collection of FluentError instances to collect errors during translation. Default is null.</param>
    /// <returns>The translated message in the preferred locale, or an English fall-back translation if the message ID is not found in the preferred locale. Returns an empty string if the message ID is not found in any locale.</returns>
    /// <exception cref="ArgumentException">Thrown if 'id' is null or empty.</exception>
    /// <example>
    /// <code>
    /// var translationService = new TranslationService(/* message contexts */);
    /// string messageId = "example-message";
    /// string translatedText = translationService.GetString(messageId);
    /// </code>
    /// </example>
    public string GetString(string id, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null)
        => GetString(id, PreferredLocale, args, errors);

    /// <summary>
    /// Gets the translated string for a given ID and, optionally, an attribute.
    /// </summary>
    /// <param name="id">The ID of the translation entry.</param>
    /// <param name="attribute">The attribute of the translation entry (optional).</param>
    /// <param name="args">The arguments to be passed to the translation (optional).</param>
    /// <param name="errors">A collection of errors that occurred during the translation (optional).</param>
    /// <returns>The translated string for the given ID and attribute in the specified locale.</returns>
    /// <exception cref="ArgumentException">Thrown if 'id' is null or empty.</exception>
    public string GetAttrString(string id, string attribute, IDictionary<string, object>? args = null, ICollection<FluentError>? errors = null)
        => GetString(id, attribute, "en", args, errors);

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