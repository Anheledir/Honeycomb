using BaseBotService.Utilities.Attributes;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using Discord.WebSocket;
using Serilog;
using Discord.Interactions;
using Discord;

namespace BaseBotService.Tests.Utilities.Attributes;

[TestFixture]
public class ArtistsOnlyAttributeTests
{
    private ArtistsOnlyAttribute _attribute;
    private ILogger _logger;
    private IGuildRepository _guildRepository;
    private IComponentInteraction _interaction;
    private IInteractionContext _context;
    private ICommandInfo _commandInfo;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger>();
        _guildRepository = Substitute.For<IGuildRepository>();
        _interaction = Substitute.For<IComponentInteraction>();
        _context = Substitute.For<IInteractionContext>();
        _commandInfo = Substitute.For<ICommandInfo>();

        _context.Interaction.Returns(_interaction);

        _attribute = new ArtistsOnlyAttribute(_logger, _guildRepository);
    }

    [Test]
    public async Task CheckRequirementsAsync_ContextNotWithinGuild_ReturnsError()
    {
        PreconditionResult result = await _attribute.CheckRequirementsAsync(_context, _commandInfo, services: null!);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorReason, Is.EqualTo("Context is not within a guild."));
        });
    }

    // Similarly, you can add tests for other conditions, such as Guild not found, No artist roles configured, etc.

    [Test]
    public async Task CheckRequirementsAsync_UserIsNotGuildUser_ReturnsError()
    {
        _interaction.GuildId.Returns((ulong?)1);
        _guildRepository.GetGuild(1).Returns(new GuildHC { ArtistRoles = new List<ulong> { 123 } });

        PreconditionResult result = await _attribute.CheckRequirementsAsync(_context, _commandInfo, services: null!);
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorReason, Is.EqualTo("User not recognized as guild user."));
        });
    }

    //[Test]
    //public async Task CheckRequirementsAsync_UserHasNoRecognizedArtistRole_ReturnsError()
    //{
    //    _interaction.GuildId.Returns(1);
    //    _guildRepository.GetGuild(1).Returns(new GuildHC { ArtistRoles = new List<ulong> { 123 } });
    //    var guildUser = Substitute.For<SocketGuildUser>();
    //    guildUser.Roles.Returns(new List<SocketRole> { new SocketRole { Id = 456 } });
    //    _context.User.Returns(guildUser);

    //    PreconditionResult result = await _attribute.CheckRequirementsAsync(_context, _commandInfo, services: null!);
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result.IsSuccess, Is.False);
    //        Assert.That(result.ErrorReason, Is.EqualTo("User has no recognized artist role."));
    //    });
    //}

    //[Test]
    //public async Task CheckRequirementsAsync_UserHasRecognizedArtistRole_ReturnsSuccess()
    //{
    //    _interaction.GuildId.Returns(1);
    //    _guildRepository.GetGuild(1).Returns(new GuildHC { ArtistRoles = new List<ulong> { 123 } });
    //    var guildUser = Substitute.For<SocketGuildUser>();
    //    guildUser.Roles.Returns(new List<SocketRole> { new SocketRole { Id = 123 } });
    //    _context.User.Returns(guildUser);

    //    PreconditionResult result = await _attribute.CheckRequirementsAsync(_context, _commandInfo, services: null!);

    //    Assert.That(result.IsSuccess, Is.True);
    //}
}