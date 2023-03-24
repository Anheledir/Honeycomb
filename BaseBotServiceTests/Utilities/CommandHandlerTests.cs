using BaseBotService.Attributes;
using BaseBotService.Utilities;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace BaseBotService.Tests.Utilities;

[TestFixture]
public class CommandHandlerTests
{
    private ILogger loggerMock;
    private ITestInterface testClassMock;
    private Assembly assembly;

    [SetUp]
    public void Setup()
    {
        loggerMock = Substitute.For<ILogger>();
        testClassMock = Substitute.For<TestClass>();
        assembly = Assembly.GetExecutingAssembly();
    }

    [Test]
    public async Task ExecuteCommand_WithValidCommand_CallsExpectedMethod()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var commandHandler = new CommandHandler(loggerMock, cache);

        // Populate the cache with a method map for testing
        var testCommandName = "test-command";
        cache.Set("methodMap", CommandHandler.BuildMethodMap(assembly));

        // Act
        await commandHandler.ExecuteCommand(testCommandName);

        // Assert
        // Verify that the expected method was called
        testClassMock.Received().TestMethod();
    }

    [Test]
    public async Task ExecuteCommand_WithInvalidCommand_LogsError()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var commandHandler = new CommandHandler(loggerMock, cache);

        // Act
        await commandHandler.ExecuteCommand("invalid-command");

        // Assert
        loggerMock.Received().Error(Arg.Any<string>());
    }

    [Test]
    public async Task ExecuteCommand_WithValidCommandAndArguments_CallsExpectedMethodWithConvertedArguments()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var commandHandler = new CommandHandler(loggerMock, cache);
        var testCommandName = "test-command2";
        cache.Set("methodMap", CommandHandler.BuildMethodMap(assembly));

        // Define the arguments to pass to the method
        string argument1 = "test";
        int argument2 = 123;
        var arguments = new object[] { argument1, argument2 };

        // Act
        await commandHandler.ExecuteCommand(testCommandName, arguments);

        // Assert
        // Verify that the expected method was called with the converted arguments
        testClassMock.Received().TestMethodWithArguments(argument1, argument2);
    }

    [Test]
    public async Task ExecuteCommand_WithValidCommandAndIncorrectArguments_LogsError()
    {
        // Arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var commandHandler = new CommandHandler(loggerMock, cache);
        var testCommandName = "test-command2";
        cache.Set("methodMap", CommandHandler.BuildMethodMap(assembly));

        // Define the arguments to pass to the method (incorrect types)
        string argument1 = "test argument";
        string argument2 = "wrong argument";
        var arguments = new object[] { argument1, argument2 };

        // Act
        await commandHandler.ExecuteCommand(testCommandName, arguments);

        // Assert
        loggerMock.Received().Error(Arg.Any<Exception>(), Arg.Any<string>());
    }

    [Test]
    public void BuildMethodMap_ReturnsDictionaryWithCommandNamesAndMethods()
    {
        // Arrange

        // Act
        Dictionary<string, MethodInfo> methodMap = CommandHandler.BuildMethodMap(assembly);

        // Assert
        Assert.That(methodMap, Is.Not.Null);
        Assert.That(methodMap.ContainsKey("test-command"), Is.True);
        Assert.That(methodMap["test-command"].DeclaringType, Is.EqualTo(typeof(TestClass)));
        Assert.That(methodMap["test-command"].Name, Is.EqualTo("TestMethod"));
        Assert.That(methodMap.ContainsKey("test-command2"), Is.True);
        Assert.That(methodMap["test-command2"].DeclaringType, Is.EqualTo(typeof(TestClass)));
        Assert.That(methodMap["test-command2"].Name, Is.EqualTo("TestMethodWithArguments"));
    }

    [Test]
    public void ConvertArguments_ConvertsArgumentsToTargetTypes()
    {
        // Arrange
        var arguments = new object[] { "arg1", "2" };
        var types = new Type[] { typeof(string), typeof(int) };
        var expectedConvertedArguments = new object[] { "arg1", 2 };

        // Act
        var result = CommandHandler.ConvertArguments(arguments, types);

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
