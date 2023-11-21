using NetArchTest.Rules;

namespace BaseBotService.Tests.Architecture;

[TestFixture]
internal class NamingConventions
{
    [Test]
    public void AllInterfaces_ShouldStartWIthPrefix_I()
    {
        var result = Types.InCurrentDomain()
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(result.GetFailingTypes());
    }

    [Test]
    public void AllClasses_WithSuffixBase_ShouldBeAbstract()
    {
        var result = Types.InCurrentDomain()
            .That()
            .ResideInNamespace("BaseBotService")
            .And()
            .HaveNameEndingWith("Base")
            .Should()
            .BeAbstract()
            .GetResult();

        result.IsSuccessful.ShouldBeTrue(result.GetFailingTypes());
    }
}