using BaseBotService.Utilities;

namespace BaseBotService.Tests.Utilities;

[TestFixture]
public class TranslationHelperTests
{
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker();
    }

    [Test]
    public void Arguments_ValidInput_ReturnsDictionary()
    {
        string name = _faker.Name.FirstName();
        object value = _faker.Random.Int();
        string name2 = _faker.Name.LastName();
        object value2 = _faker.Random.Int();

        Dictionary<string, object> args = TranslationHelper.Arguments(name, value, name2, value2);

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

        _ = Should.Throw<ArgumentNullException>(() => TranslationHelper.Arguments(name, value));
    }

    [Test]
    public void Arguments_InvalidInputCount_ThrowsException()
    {
        string name = _faker.Name.FirstName();
        object value = _faker.Random.Int();
        string name2 = _faker.Name.FirstName();

        _ = Should.Throw<ArgumentException>(() => TranslationHelper.Arguments(name, value, name2));
    }
}
