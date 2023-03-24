using Discord;

namespace BaseBotServiceTests.Utilities;

public static class GuildFactory
{
    public static IGuild CreateMockGuild()
    {
        Faker faker = new();

        IGuild guild = Substitute.For<IGuild>();
        _ = guild.Id.Returns(faker.Random.ULong());
        _ = guild.Name.Returns(faker.Company.CompanyName());

        // Set additional properties or methods if necessary.

        return guild;
    }
}
