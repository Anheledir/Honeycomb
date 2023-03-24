using Discord;

namespace BaseBotService.Tests;

public static class UserFactory
{
    public static IUser CreateMockUser(bool isBot = false, bool isWebhook = false, ulong? id = null)
    {
        Faker faker = new();

        IUser user = Substitute.For<IUser>();

        _ = user.Id.Returns(id ?? faker.Random.ULong());
        _ = user.Username.Returns(faker.Internet.UserName());
        _ = user.Discriminator.Returns(faker.Random.Int(1000, 9999).ToString("D4"));
        _ = user.IsBot.Returns(isBot);
        _ = user.IsWebhook.Returns(isWebhook);
        _ = user.CreatedAt.Returns(faker.Date.Past(2));

        return user;
    }
}
