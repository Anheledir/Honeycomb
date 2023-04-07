using BaseBotService.Utilities.Extensions;

namespace BaseBotService.Tests.Utilities.Extensions;

[TestFixture]
public class ListExtensionsTests
{
    private List<int> _intList;
    private Faker _faker;

    [SetUp]
    public void SetUp()
    {
        _faker = new Faker();
        _intList = new List<int>();
        for (int i = 0; i < _faker.Random.Int(5, 10); i++)
        {
            _intList.Add(_faker.Random.Int());
        }
    }

    [Test]
    public void GetRandomItem_GivenNonEmptyList_ShouldReturnRandomItemFromList()
    {
        int randomItem = _intList.GetRandomItem();

        _intList.ShouldContain(randomItem);
    }

    [Test]
    public void GetRandomItem_GivenEmptyList_ShouldThrowArgumentException()
    {
        _intList.Clear();

        Should.Throw<ArgumentException>(() => _intList.GetRandomItem());
    }

    [Test]
    public void GetRandomItem_GivenNullList_ShouldThrowArgumentException()
    {
        List<int>? nullList = default;

        Should.Throw<ArgumentException>(() => nullList!.GetRandomItem());
    }
}