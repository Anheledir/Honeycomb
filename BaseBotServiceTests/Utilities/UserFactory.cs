using Discord;

namespace BaseBotService.Tests.Utilities
{
    public static class UserFactory
    {
        public static IUser CreateMockUser(bool isBot = false, bool isWebhook = false, ulong? id = null)
        {
            var faker = new Faker();

            var user = Substitute.For<IUser>();

            user.Id.Returns(id ?? faker.Random.ULong());
            user.Username.Returns(faker.Internet.UserName());
            user.Discriminator.Returns(faker.Random.Int(1000, 9999).ToString("D4"));
            user.IsBot.Returns(isBot);
            user.IsWebhook.Returns(isWebhook);
            user.CreatedAt.Returns(faker.Date.Past(2));

            return user;
        }
    }
}
