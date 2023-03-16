using Discord;

namespace BaseBotService.Tests.Utilities
{
    public static class GuildFactory
    {
        public static IGuild CreateMockGuild()
        {
            var faker = new Faker();

            var guild = Substitute.For<IGuild>();
            guild.Id.Returns(faker.Random.ULong());
            guild.Name.Returns(faker.Company.CompanyName());

            // Set additional properties or methods if necessary.

            return guild;
        }
    }
}
