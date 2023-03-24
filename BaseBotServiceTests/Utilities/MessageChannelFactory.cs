using Discord;

namespace BaseBotServiceTests.Utilities;

public static class MessageChannelFactory
{
    public static IMessageChannel CreateMockMessageChannel()
    {
        IMessageChannel messageChannel = Substitute.For<IMessageChannel>();
        // Set additional properties or methods if necessary.
        return messageChannel;
    }
}
