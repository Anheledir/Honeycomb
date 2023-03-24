using BaseBotService.Core;
using BaseBotService.Utilities.Attributes;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace BaseBotService.Tests.Core;

[TestFixture]
public class CommandHandlerTests
{
    private ILogger _loggerMock;
    private ITestInterface _testClassMock;
    private Assembly _assembly;

    [SetUp]
    public void Setup()
    {
        _loggerMock = Substitute.For<ILogger>();
        _testClassMock = Substitute.For<TestClass>();
        _assembly = Assembly.GetExecutingAssembly();
    }

    [Test]
    public async Task ExecuteCommand_WithValidCommand_CallsExpectedMethod()
    {
        // Arrange
        MemoryCache cache = new(new MemoryCacheOptions());
        CommandHandler commandHandler = new(_loggerMock, cache);

        // Populate the cache with a method map for testing
        const string testCommandName = "test-command";
        _ = cache.Set("methodMap", CommandHandler.BuildMethodMap(_assembly));

        // Act
        await commandHandler.ExecuteCommand(testCommandName);

        // Assert
        // Verify that the expected method was called
        _testClassMock.Received().TestMethod();
    }

    [Test]
    public async Task ExecuteCommand_WithInvalidCommand_LogsError()
    {
        // Arrange
        MemoryCache cache = new(new MemoryCacheOptions());
        CommandHandler commandHandler = new(_loggerMock, cache);

        // Act
        await commandHandler.ExecuteCommand("invalid-command");

        // Assert
        _loggerMock.Received().Error(Arg.Any<string>());
    }

    [Test]
    public async Task ExecuteCommand_WithValidCommandAndArguments_CallsExpectedMethodWithConvertedArguments()
    {
        // Arrange
        MemoryCache cache = new(new MemoryCacheOptions());
        CommandHandler commandHandler = new(_loggerMock, cache);
        const string testCommandName = "test-command2";
        _ = cache.Set("methodMap", CommandHandler.BuildMethodMap(_assembly));

        // Define the arguments to pass to the method
        const string argument1 = "test";
        const int argument2 = 123;
        object[] arguments = new object[] { argument1, argument2 };

        // Act
        await commandHandler.ExecuteCommand(testCommandName, arguments);

        // Assert
        // Verify that the expected method was called with the converted arguments
        _testClassMock.Received().TestMethodWithArguments(argument1, argument2);
    }

    [Test]
    public async Task ExecuteCommand_WithValidCommandAndIncorrectArguments_LogsError()
    {
        // Arrange
        MemoryCache cache = new(new MemoryCacheOptions());
        CommandHandler commandHandler = new(_loggerMock, cache);
        const string testCommandName = "test-command2";
        _ = cache.Set("methodMap", CommandHandler.BuildMethodMap(_assembly));

        // Define the arguments to pass to the method (incorrect types)
        const string argument1 = "test argument";
        const string argument2 = "wrong argument";
        object[] arguments = new object[] { argument1, argument2 };

        // Act
        await commandHandler.ExecuteCommand(testCommandName, arguments);

        // Assert
        _loggerMock.Received().Error(Arg.Any<Exception>(), Arg.Any<string>());
    }

    [Test]
    public void BuildMethodMap_ReturnsDictionaryWithCommandNamesAndMethods()
    {
        // Arrange

        // Act
        Dictionary<string, MethodInfo> methodMap = CommandHandler.BuildMethodMap(_assembly);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(methodMap, Is.Not.Null);
            Assert.That(methodMap.ContainsKey("test-command"), Is.True);
            Assert.That(methodMap["test-command"].DeclaringType, Is.EqualTo(typeof(TestClass)));
            Assert.That(methodMap["test-command"].Name, Is.EqualTo("TestMethod"));
            Assert.That(methodMap.ContainsKey("test-command2"), Is.True);
            Assert.That(methodMap["test-command2"].DeclaringType, Is.EqualTo(typeof(TestClass)));
            Assert.That(methodMap["test-command2"].Name, Is.EqualTo("TestMethodWithArguments"));
        });
    }

    [Test]
    public void ConvertArguments_ConvertsArgumentsToTargetTypes()
    {
        // Arrange
        object[] arguments = new object[] { "arg1", "2" };
        Type[] types = new Type[] { typeof(string), typeof(int) };
        object[] expectedConvertedArguments = new object[] { "arg1", 2 };

        // Act
        object[] result = CommandHandler.ConvertArguments(arguments, types);

        // Assert
        Assert.That(result, Is.EqualTo(expectedConvertedArguments));
    }
}

public interface ITestInterface
{
    void TestMethod();
    void TestMethodWithArguments(string arg1, int arg2);
}

public class TestClass : ITestInterface
{
    [Command("test-command")]
    public void TestMethod()
    {
        // Do something
    }

    [Command("test-command2", typeof(string), typeof(int))]
    public void TestMethodWithArguments(string arg1, int arg2)
    {
        // Do something with the arguments
    }
}
