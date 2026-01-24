using BaseBotService.Core.Interfaces;
using BaseBotService.Infrastructure.Services;
using Serilog;

namespace BaseBotService.Tests.Infrastructure.Services;

[TestFixture]
public class EnvironmentServiceTests
{
    private string? _originalBotToken;
    private string? _originalHealthPort;

    [SetUp]
    public void SetUp()
    {
        _originalBotToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN");
        _originalHealthPort = Environment.GetEnvironmentVariable("HEALTH_PORT");
    }

    [TearDown]
    public void TearDown()
    {
        Environment.SetEnvironmentVariable("DISCORD_BOT_TOKEN", _originalBotToken);
        Environment.SetEnvironmentVariable("HEALTH_PORT", _originalHealthPort);
    }

    [Test]
    public void Constructor_WhenHealthPortIsInvalid_UsesDefaultPort()
    {
        Environment.SetEnvironmentVariable("DISCORD_BOT_TOKEN", "test-token");
        Environment.SetEnvironmentVariable("HEALTH_PORT", "not-a-number");

        var logger = Substitute.For<ILogger>();
        var translationService = Substitute.For<ITranslationService>();
        var cancellationTokenSource = new CancellationTokenSource();

        var service = new EnvironmentService(logger, translationService, cancellationTokenSource);

        service.HealthPort.ShouldBe(8080);
    }

    [Test]
    public void Constructor_WhenHealthPortIsValid_UsesConfiguredPort()
    {
        Environment.SetEnvironmentVariable("DISCORD_BOT_TOKEN", "test-token");
        Environment.SetEnvironmentVariable("HEALTH_PORT", "9001");

        var logger = Substitute.For<ILogger>();
        var translationService = Substitute.For<ITranslationService>();
        var cancellationTokenSource = new CancellationTokenSource();

        var service = new EnvironmentService(logger, translationService, cancellationTokenSource);

        service.HealthPort.ShouldBe(9001);
    }
}
