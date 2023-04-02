using BaseBotService.Infrastructure.Services;
using Fluent.Net;

namespace BaseBotService.Tests.Infrastructure.Services;

public class TranslationServiceTests
{
    private TranslationService _translationService;
    private IEnumerable<MessageContext> _messageContexts;
    private const string _messageId = "test-message";
    private const string _attribute = "alternative";
    private const string _englishTranslation = "Hello, world!";
    private const string _germanTranslation = "Hallo, Welt!";
    private const string _spanishTranslation = "¡Hola, mundo!";
    private const string _frenchTranslation = "Bonjour, le monde!";
    private const string _englishTranslationAlt = "English Greeting";
    private const string _germanTranslationAlt = "Deutsche Begrüßung";
    private const string _spanishTranslationAlt = "¡Hola, mundo alternativo!";
    private const string _frenchTranslationAlt = "Bonjour, le différent monde!";

    [SetUp]
    public void Setup()
    {
        _messageContexts = CreateMessageContexts();
        _translationService = new TranslationService(_messageContexts);
    }

    private IEnumerable<MessageContext> CreateMessageContexts()
    {
        var locales = new[] { "en", "de", "es", "fr" };
        var messageContexts = new List<MessageContext>();

        foreach (var locale in locales)
        {
            var context = new MessageContext(new[] { locale });
            var testFtlFile = $"BaseBotService.Tests.Resources.{locale}.ftl";
            var resource = LoadResource(testFtlFile);
            context.AddResource(resource);
            messageContexts.Add(context);
        }

        return messageContexts;
    }

    private FluentResource LoadResource(string resourceName)
    {
        using Stream stream = GetType().Assembly.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return FluentResource.FromReader(reader);
    }

    [Test]
    public void GetString_ValidId_ReturnsTranslation()
    {
        string expectedTranslation = _translationService.GetString(_messageId);

        _translationService.PreferredLocale.ShouldBe("en");
        expectedTranslation.ShouldNotBeNullOrEmpty();
        expectedTranslation.ShouldBeEquivalentTo(_englishTranslation);
    }

    [Test]
    public void GetString_ValidIdAndAttribute_ReturnsTranslation()
    {
        string expectedTranslation = _translationService.GetAttrString(_messageId, _attribute);

        _translationService.PreferredLocale.ShouldBe("en");
        expectedTranslation.ShouldNotBeNullOrEmpty();
        expectedTranslation.ShouldBeEquivalentTo(_englishTranslationAlt);
    }

    [TestCase("en", _englishTranslation)]
    [TestCase("de", _germanTranslation)]
    [TestCase("es", _spanishTranslation)]
    [TestCase("fr", _frenchTranslation)]
    public void GetString_ValidId_WithLocale_ReturnsTranslation(string locale, string translation)
    {
        string expectedTranslation = _translationService.GetString(_messageId, locale);

        expectedTranslation.ShouldNotBeNullOrEmpty();
        expectedTranslation.ShouldBeEquivalentTo(translation);
    }

    [TestCase("en", _englishTranslationAlt)]
    [TestCase("de", _germanTranslationAlt)]
    [TestCase("es", _spanishTranslationAlt)]
    [TestCase("fr", _frenchTranslationAlt)]
    public void GetAttrString_WithValidIdAndAttribute_ReturnsExpectedTranslation(string locale, string translation)
    {
        string result = _translationService.GetString(_messageId, _attribute, locale);
        Assert.That(result, Is.EqualTo(translation));
    }

    [Test]
    public void GetString_InvalidId_ReturnsEmptyString()
    {
        string translation = _translationService.GetString(string.Concat(_messageId, "-non-existing"));

        translation.ShouldBeEmpty();
    }

    [Test]
    public void PreferredLocale_ReturnsFirstLocale()
    {
        string preferredLocale = _translationService.PreferredLocale;

        preferredLocale.ShouldNotBeNullOrEmpty();
    }

    [Test]
    public void Culture_ReturnsCultureInfo()
    {
        System.Globalization.CultureInfo culture = _translationService.Culture;

        _ = culture.ShouldNotBeNull();
        culture.Name.ShouldBe(_translationService.PreferredLocale);
    }
}