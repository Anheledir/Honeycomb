using Fluent.Net;
using System.Reflection;

namespace BaseBotService.Infrastructure;

/// <summary>
/// A factory class responsible for creating and managing translation-related objects, such as MessageContext instances.
/// </summary>
public static class TranslationFactory
{
    /// <summary>
    /// Creates a collection of MessageContext instances for the given locales with the appropriate resource files.
    /// </summary>
    /// <returns>A collection of MessageContext instances with the loaded resources.</returns>
    public static IEnumerable<MessageContext> CreateMessageContexts()
    {
        var locales = new[] { "en", "de", "es", "fr" };
        var messageContexts = new List<MessageContext>();

        foreach (var locale in locales)
        {
            var context = new MessageContext(new[] { locale });
            var resourceName = $"BaseBotService.Locales.{locale}.ftl";
            var resource = LoadResource(resourceName);
            context.AddResource(resource);
            messageContexts.Add(context);
        }

        return messageContexts;
    }

    /// <summary>
    /// Loads a FluentResource from the specified embedded resource file.
    /// </summary>
    /// <param name="resourceName">The name of the embedded resource file, including the namespace.</param>
    /// <returns>A FluentResource instance containing the parsed content of the resource file.</returns>
    private static FluentResource LoadResource(string resourceName)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return FluentResource.FromReader(reader);
    }
}
