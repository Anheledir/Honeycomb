using Discord.Commands;
using Discord.WebSocket;
using PreconditionAttribute = Discord.Commands.PreconditionAttribute;
using PreconditionResult = Discord.Commands.PreconditionResult;

namespace BaseBotService.Utilities.Attributes;

/// <summary>
/// A precondition attribute that checks if the user interacting with a Discord component matches the user ID embedded in the component's custom ID.
/// </summary>
internal class DoUserCheckAttribute : PreconditionAttribute
{
    /// <summary>
    /// Checks whether the user interacting with the component matches the expected user ID.
    /// </summary>
    /// <param name="context">The interaction context.</param>
    /// <param name="command">The command information.</param>
    /// <param name="services">The service provider for resolving dependencies.</param>
    /// <returns>A Task representing the asynchronous operation, containing the result of the user check.</returns>
    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.User is not SocketGuildUser user)
        {
            return Task.FromResult(PreconditionResult.FromError("This command can only be executed in a guild context."));
        }

        if (context is not IInteractionContext interactionContext || interactionContext.Interaction is not SocketMessageComponent componentContext)
        {
            return Task.FromResult(PreconditionResult.FromError("Context unrecognized as component context."));
        }

        if (TryGetUserIdFromCustomId(componentContext.Data.CustomId, out ulong id))
        {
            return (user.Id == id)
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError("User ID does not match component ID!"));
        }

        return Task.FromResult(PreconditionResult.FromError("Unable to parse user ID from custom ID."));
    }

    /// <summary>
    /// Attempts to extract the user ID from the custom ID of the component.
    /// </summary>
    /// <param name="customId">The custom ID string to parse.</param>
    /// <param name="userId">The extracted user ID.</param>
    /// <returns>True if the user ID was successfully extracted; otherwise, false.</returns>
    private bool TryGetUserIdFromCustomId(string customId, out ulong userId)
    {
        userId = 0;
        string[] param = customId.Split(':');

        if (param.Length > 1)
        {
            string userIdString = param[1].Split(',')[0];
            return ulong.TryParse(userIdString, out userId);
        }

        return false;
    }
}
