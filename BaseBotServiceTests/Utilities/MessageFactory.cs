using Discord;

namespace BaseBotService.Tests.Utilities
{
    public static class MessageFactory
    {
        public static IMessage CreateMockMessage(
            string? content = "",
            bool isBot = false,
            bool isWebhook = false,
            ulong? authorId = null
            )
        {
            var faker = new Faker();

            var message = Substitute.For<IMessage>();
            message.CreatedAt.Returns(faker.Date.Past(2));
            message.Channel.Returns(MessageChannelFactory.CreateMockMessageChannel());
            message.Author.Returns(_ => UserFactory.CreateMockUser(isBot, isWebhook, authorId));
            message.Content.Returns(content ?? faker.Lorem.Sentence());
            message.Id.Returns(faker.Random.ULong());
            message.Timestamp.Returns(faker.Date.RecentOffset());

            return message;
        }
    }
}
