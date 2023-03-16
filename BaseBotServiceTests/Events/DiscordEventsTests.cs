using BaseBotService.Enumeration;
using BaseBotService.Events;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using BaseBotService.Tests.Utilities;
using Discord;
using Serilog.Events;

namespace BaseBotService.Tests.Events;

[TestFixture]
public class DiscordEventsTests
{
    private ILogger _substituteLogger;
    private DiscordSocketClient _substituteDiscordSocketClient;
    private IServiceProvider _substituteServiceProvider;
    private IAssemblyService _substituteAssemblyService;
    private IEnvironmentService _substituteEnvironmentService;
    private IEngagementService _substituteEngagementService;
    private InteractionService _substituteInteractionService;

    [SetUp]
    public void SetUp()
    {
        _substituteLogger = Substitute.For<ILogger>();
        _substituteDiscordSocketClient = Substitute.For<DiscordSocketClient>();
        _substituteServiceProvider = Substitute.For<IServiceProvider>();
        _substituteAssemblyService = Substitute.For<IAssemblyService>();
        _substituteEnvironmentService = Substitute.For<IEnvironmentService>();
        _substituteEngagementService = Substitute.For<IEngagementService>();
        _substituteInteractionService = Substitute.For<InteractionService>(_substituteDiscordSocketClient, null);
    }

    [Test]
    public async Task DisconnectedAsync_ShouldLogWarning()
    {
        // Arrange
        DiscordEvents discordEvents = CreateDiscordEventsMock();

        var exception = new Exception("Test exception");

        // Act
        await discordEvents.DisconnectedAsync(exception);

        // Assert
        _substituteLogger.Received().Warning(exception, "Lost connection to Discord.");
    }

    [Test]
    public async Task LogAsync_ShouldLogMessageWithSerilog()
    {
        // Arrange
        var faker = new Faker();
        var logMessage = new LogMessage(faker.PickRandom<LogSeverity>(), faker.Random.Word(), faker.Lorem.Sentence(), faker.System.Exception());

        var discordEvents = CreateDiscordEventsMock();

        // Act
        await discordEvents.LogAsync(logMessage);

        // Assert
        _substituteLogger.Received().Write(
            Arg.Is<LogEventLevel>(level => level == logMessage.GetSerilogSeverity()),
            logMessage.Exception,
            "[{Source}] {Message}",
            logMessage.Source,
            logMessage.Message);
    }

    [Test]
    public async Task MessageReceived_ShouldHandleDifferentMessageTypes()
    {
        // Arrange
        var discordEvents = CreateDiscordEventsMock();

        Faker faker = new();
        var userMessageInDm = MessageFactory.CreateMockMessage(null, false, false);
        var dmChannel = Substitute.For<IMessageChannel, IDMChannel>();

        ulong userId = faker.Random.ULong();
        var userMessageInGuild = MessageFactory.CreateMockMessage(null, false, false, userId);
        var guildChannel = Substitute.For<IMessageChannel, IGuildChannel>();
        ulong guildId = faker.Random.ULong();
        ((IGuildChannel)guildChannel).GuildId.Returns(guildId);

        userMessageInDm.Channel.Returns(dmChannel);
        userMessageInGuild.Channel.Returns(guildChannel);

        // Act
        await discordEvents.MessageReceived(userMessageInDm);
        await discordEvents.MessageReceived(userMessageInGuild);

        // Assert
        _substituteLogger.Received().Debug(Arg.Any<string>()); // user message within DM
        await _substituteEngagementService.Received().AddActivityTick(guildId, userId); // user message within guild
    }

    [TestCase(RegisterCommandsOnStartup.NoRegistration, "Skipping global application command registration.")]
    public async Task ReadyAsync_ShouldCallExpectedMethods(RegisterCommandsOnStartup registerCommands, string registerCommandsLog)
    {
        // Arrange
        var environmentService = Substitute.For<IEnvironmentService>();
        environmentService.EnvironmentName.Returns("TestEnvironment");
        environmentService.RegisterCommands.Returns(registerCommands);

        var assemblyService = Substitute.For<IAssemblyService>();
        assemblyService.Name.Returns("TestAssembly");
        assemblyService.Version.Returns("1.2.3");

        var discordEvents = CreateDiscordEventsMock(assemblyService: assemblyService, environmentService: environmentService);

        // Act
        await discordEvents.ReadyAsync();

        // Assert
        _substituteLogger.Received().Information(Arg.Is<string>(s => s.Contains("TestAssembly") && s.Contains("1.2.3") && s.Contains("TestEnvironment")));
        await _substituteDiscordSocketClient.Received().SetActivityAsync(Arg.Any<Game>());
        await _substituteDiscordSocketClient.Received().SetStatusAsync(UserStatus.Online);
        _substituteLogger.Received().Information(registerCommandsLog);
    }

    private DiscordEvents CreateDiscordEventsMock(
        ILogger? logger = null,
        DiscordSocketClient? discordSocketClient = null,
        IServiceProvider? serviceProvider = null,
        IAssemblyService? assemblyService = null,
        IEnvironmentService? environmentService = null,
        IEngagementService? engagementService = null,
        InteractionService? interactionService = null)
    => new(
        logger ?? _substituteLogger,
        discordSocketClient ?? _substituteDiscordSocketClient,
        serviceProvider ?? _substituteServiceProvider,
        assemblyService ?? _substituteAssemblyService,
        environmentService ?? _substituteEnvironmentService,
        engagementService ?? _substituteEngagementService,
        interactionService ?? _substituteInteractionService
    );

}