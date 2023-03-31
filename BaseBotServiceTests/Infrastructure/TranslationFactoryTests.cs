using BaseBotService.Infrastructure;
using Fluent.Net;

namespace BaseBotService.Tests.Infrastructure;

public class TranslationFactoryTests
{
    [Test]
    public void CreateMessageContexts_CreatesExpectedNumberOfContexts()
    {
        var expectedLocales = new[] { "en", "de", "es", "fr" };
        var expectedContextCount = expectedLocales.Length;

        IEnumerable<MessageContext> contexts = TranslationFactory.CreateMessageContexts();

        Assert.That(contexts.Count(), Is.EqualTo(expectedContextCount));
    }

    [Test]
    public void CreateMessageContexts_ContextsHaveCorrectLocales()
    {
        var expectedLocales = new[] { "en", "de", "es", "fr" };

        IEnumerable<MessageContext> contexts = TranslationFactory.CreateMessageContexts();
        var actualLocales = contexts.Select(context => context.Locales.First()).ToArray();

        Assert.That(actualLocales, Is.EqualTo(expectedLocales));
    }
}
