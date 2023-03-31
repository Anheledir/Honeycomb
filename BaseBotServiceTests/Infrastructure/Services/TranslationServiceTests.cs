using BaseBotService.Infrastructure.Services;
using Fluent.Net;

namespace BaseBotService.Tests.Infrastructure.Services;

public class TranslationServiceTests
{
    private TranslationService _translationService;
    private IEnumerable<MessageContext> _messageContexts;
    private Faker _faker;
    private const string _messageId = "test-message";
    private const string _englishTranslation = "Hello, world!";
    private const string _germanTranslation = "Hallo, Welt!";
    private const string _spanishTranslation = "¡Hola, mundo!";
    private const string _frenchTranslation = "Bonjour, le monde!";

    [SetUp]
    public void Setup()
    {
        _faker = new Faker();
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

    [Test]
    public void GetString_InvalidId_ReturnsEmptyString()
    {
        string translation = _translationService.GetString(string.Concat(_messageId, "-non-existing"));

        translation.ShouldBeEmpty();
    }

    [Test]
    public void Arguments_ValidInput_ReturnsDictionary()
    {
        string name = _faker.Name.FirstName();
        object value = _faker.Random.Int();
        string name2 = _faker.Name.LastName();
        object value2 = _faker.Random.Int();

        Dictionary<string, object> args = _translationService.Arguments(name, value, name2, value2);

        _ = args.ShouldNotBeNull();
        args.Count.ShouldBe(2);
        args[name].ShouldBe(value);
        args[name2].ShouldBe(value2);
    }

    [Test]
    public void Arguments_InvalidInput_ThrowsException()
    {
        string name = string.Empty;
        object value = _faker.Random.Int();

        _ = Should.Throw<ArgumentNullException>(() => _translationService.Arguments(name, value));
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