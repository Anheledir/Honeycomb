using BaseBotService.Core.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PreconditionAttribute = Discord.Commands.PreconditionAttribute;
using PreconditionResult = Discord.Commands.PreconditionResult;

namespace BaseBotService.Utilities.Attributes;

/// <summary>
/// A custom precondition attribute that checks if the user has the required moderator role to execute a command.
/// </summary>
public class RequireModeratorRoleAttribute : PreconditionAttribute
{
    /// <summary>
    /// Checks whether the user has the necessary permissions to execute the command.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="command">The command information.</param>
    /// <param name="services">The service provider for resolving dependencies.</param>
    /// <returns>A Task that represents the asynchronous operation, containing the result of the permission check.</returns>
    public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        var permissionService = services.GetRequiredService<IPermissionService>();
        var user = context.User as SocketGuildUser;

        // Ensure the command is being executed in a guild and the user is not null
        if (user == null || context.Guild == null)
        {
            return PreconditionResult.FromError("This command can only be executed within a guild.");
        }

        try
        {
            bool hasPermission = await permissionService.CanUserExecuteModeratorCommandAsync(user);

            return hasPermission
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError("You do not have permission to run this command.");
        }
        catch (Exception ex)
        {
            // Log the exception (you can replace this with your logging mechanism)
            Console.WriteLine($"An error occurred while checking permissions: {ex.Message}");
            return PreconditionResult.FromError("An error occurred while checking your permissions. Please try again later.");
        }
    }
}
