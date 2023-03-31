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
    /// Creates a dictionary for storing arguments to be used in translations.
    /// </summary>
    /// <param name="name">The name of the first argument.</param>
    /// <param name="value">The value of the first argument.</param>
    /// <param name="args">Additional argument pairs, in name and value order.</param>
    /// <returns>A dictionary containing the given arguments.</returns>
    /// <exception cref="ArgumentNullException">Thrown when name or value is null or empty.</exception>
    /// <exception cref="ArgumentException">Thrown when the number of arguments is not a multiple of two or an argument name is empty.</exception>
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
    /// Retrieves the translated string for the given id, using the provided arguments and reporting any errors.
    /// </summary>
    /// <param name="id">The message identifier.</param>
    /// <param name="args">The arguments to be used in the translation.</param>
    /// <param name="errors">A collection for storing errors encountered during translation.</param>
    /// <returns>The translated string or an empty string if the message is not found.</returns>
    public string GetString(string id, IDictionary<string, object>? args = null,
        ICollection<FluentError>? errors = null)
    {
        foreach (MessageContext context in _contexts)
        {
            Fluent.Net.RuntimeAst.Message msg = context.GetMessage(id);
            if (msg != null)
            {
                return context.Format(msg, args, errors);
            }
        }
        return string.Empty;
    }

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