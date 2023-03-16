using Discord;

namespace BaseBotService.Tests.Utilities
{
    public static class MessageChannelFactory
    {
        public static IMessageChannel CreateMockMessageChannel()
        {
            var messageChannel = Substitute.For<IMessageChannel>();
            // Set additional properties or methods if necessary.
            return messageChannel;
        }
    }
}
