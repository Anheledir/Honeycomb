using BaseBotService.Core.Interfaces;
using BaseBotService.Infrastructure.Services;
using Fluent.Net;

namespace BaseBotService.Tests.Infrastructure.Services;

public class TranslationServiceTests
{
    private TranslationService _translationService;
    private IEnumerable<MessageContext> _messageContexts;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker();
        _messageContexts = CreateMessageContexts();
        _translationService = new TranslationService(_messageContexts);
    }

    private IEnumerable<MessageContext> CreateMessageContexts()
    {
        MessageContext context = new(new[] { "en-US" });
        const string testFtlFile = "BaseBotService.Tests.Resources.test.ftl";
        FluentResource resource = LoadResource(testFtlFile);
        _ = context.AddResource(resource);

        // Return a single context in this example
        return new List<MessageContext> { context };
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
        const string id = "test-message";
        string expectedTranslation = _translationService.GetString(id);

        expectedTranslation.ShouldNotBeNullOrEmpty();
        expectedTranslation.ShouldBeEquivalentTo("Test translation");
    }

    [Test]
    public void GetString_InvalidId_ReturnsEmptyString()
    {
        const string id = "non-existent-message";
        string translation = _translationService.GetString(id);

        translation.ShouldBeEmpty();
    }

    [Test]
    public void Arguments_ValidInput_ReturnsDictionary()
    {
        string name = _faker.Name.FirstName();
        object value = _faker.Random.Int();
        string name2 = _faker.Name.LastName();
        object value2 = _faker.Random.Int();

        Dictionary<string, object> args = TranslationService.Arguments(name, value, name2, value2);

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

        _ = Should.Throw<ArgumentNullException>(() => TranslationService.Arguments(name, value));
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