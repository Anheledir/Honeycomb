using BaseBotService.Utilities.Attributes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaseBotService.Core;
/// <summary>
/// Represents a command handler that can execute methods based on command names.
/// </summary>
public class CommandHandler
{
    private readonly ILogger _logger;
    private readonly IMemoryCache _cache;
    private static readonly ILogger _staticLogger = Program.ServiceProvider.GetRequiredService<ILogger>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandler"/> class.
    /// </summary>
    /// <param name="cache">The cache used to store the method map.</param>
    public CommandHandler(ILogger logger, IMemoryCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Executes the method associated with the specified command name and arguments.
    /// </summary>
    /// <param name="commandName">The name of the command to execute.</param>
    /// <param name="arguments">The arguments to pass to the method.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExecuteCommand(string commandName, params object[] arguments)
    {
        Dictionary<string, MethodInfo>? methodMap = _cache.GetOrCreate("methodMap", entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return BuildMethodMap();
        });
        try
        {
            if (methodMap?.TryGetValue(commandName, out MethodInfo? method) ?? false)
            {
                _logger.Debug($"Executing command '{commandName}' with arguments: {string.Join(", ", arguments)}");
                object? instance = Activator.CreateInstance(method.DeclaringType!);
                Type[] argumentTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
                object[] convertedArguments = arguments == null ? Array.Empty<object>() : ConvertArguments(arguments, argumentTypes);
                if (method != null && instance != null && convertedArguments != null)
                {
                    await (Task)method.Invoke(instance, convertedArguments)!;
                }
                else
                {
                    _logger.Error($"Tried to execute command '{commandName}', but some value got null.");
                }
            }
            else
            {
                _logger.Error($"Tried to execute command '{commandName}', but couldn't find a handler for it.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Tried to execute command '{commandName}, but an exception happened.'");
        }
    }

    /// <summary>
    /// Builds the method map by searching for methods decorated with the <see cref="CommandAttribute"/>.
    /// </summary>
    /// <returns>A dictionary that maps command names to methods.</returns>
    internal static Dictionary<string, MethodInfo> BuildMethodMap(Assembly? assembly = null)
    {
        if (assembly == null)
        {
            assembly = Assembly.GetExecutingAssembly();
        }
        _staticLogger.Information($"Building method map for assembly {assembly}.");
        Dictionary<string, MethodInfo> methodMap = new();
        foreach (Type type in assembly.GetTypes())
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                CommandAttribute? attribute = method.GetCustomAttribute<CommandAttribute>();
                if (attribute != null)
                {
                    string commandName = attribute.CommandName;
                    methodMap[commandName] = method;
                    _staticLogger.Debug($"{commandName}: {commandName}");
                }
            }
        }
        return methodMap;
    }

    /// <summary>
    /// Converts the arguments to the specified types.
    /// </summary>
    /// <param name="arguments">The arguments to convert.</param>
    /// <param name="types">The target types.</param>
    /// <returns>An array of arguments with the converted types.</returns>
    internal static object[] ConvertArguments(object[] arguments, Type[] types)
    {
        object[] convertedArguments = new object[arguments.Length];
        for (int i = 0; i < arguments.Length; i++)
        {
            convertedArguments[i] = Convert.ChangeType(arguments[i], types[i]);
        }
        return convertedArguments;
    }
}