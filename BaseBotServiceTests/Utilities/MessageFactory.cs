using Discord;

namespace BaseBotServiceTests.Utilities;

public static class MessageFactory
{
    public static IMessage CreateMockMessage(
        string? content = "",
        bool isBot = false,
        bool isWebhook = false,
        ulong? authorId = null
        )
    {
        Faker faker = new();

        IMessage message = Substitute.For<IMessage>();
        _ = message.CreatedAt.Returns(faker.Date.Past(2));
        _ = message.Channel.Returns(MessageChannelFactory.CreateMockMessageChannel());
        _ = message.Author.Returns(_ => UserFactory.CreateMockUser(isBot, isWebhook, authorId));
        _ = message.Content.Returns(content ?? faker.Lorem.Sentence());
        _ = message.Id.Returns(faker.Random.ULong());
        _ = message.Timestamp.Returns(faker.Date.RecentOffset());

        return message;
    }
}
