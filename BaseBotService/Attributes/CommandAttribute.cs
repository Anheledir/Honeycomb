namespace BaseBotService.Attributes;

/// <summary>
/// Represents an attribute that can be used to decorate methods that can be executed by a <see cref="CommandHandler"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the command associated with the method.
    /// </summary>
    public string CommandName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute"/> class with the specified command name.
    /// </summary>
    /// <param name="commandName">The name of the command associated with the method.</param>
    public CommandAttribute(string commandName)
        => CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute"/> class with the specified command name and argument types.
    /// </summary>
    /// <param name="commandName">The name of the command associated with the method.</param>
    /// <param name="argumentTypes">The types of the arguments expected by the method.</param>
    public CommandAttribute(string commandName, params Type[]? argumentTypes)
    {
        CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
        ArgumentTypes = argumentTypes;
    }

    /// <summary>
    /// Gets the types of the arguments expected by the method.
    /// </summary>
    public Type[]? ArgumentTypes { get; }
}
