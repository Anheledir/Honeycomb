using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Utilities.Attributes;

/// <summary>
/// A custom attribute for checking whether a context's user has a moderator role.
/// </summary>
internal class ModeratorsOnlyAttribute : PreconditionAttribute
{
    /// <inheritdoc/>
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        ILogger Logger = Program.ServiceProvider.GetRequiredService<ILogger>();
        IGuildRepository GuildRepository = Program.ServiceProvider.GetRequiredService<IGuildRepository>();

        if (context.Interaction is not IComponentInteraction componentContext)
        {
            return Task.FromResult(PreconditionResult.FromError("Context unrecognized as component context."));
        }

        if (!componentContext.GuildId.HasValue)
        {
            return Task.FromResult(PreconditionResult.FromError("Context is not within a guild."));
        }

        Logger.Information($"Query guild data for {componentContext.GuildId.Value}");
        GuildHC? guild = GuildRepository.GetGuild(componentContext.GuildId.Value);
        if (guild is null)
        {
            return Task.FromResult(PreconditionResult.FromError("Guild not found."));
        }

        Logger.Information($"{componentContext.GuildId.Value} has configured {guild.ModeratorRoles.Count} moderator roles.");
        if (guild.ModeratorRoles.Count == 0)
        {
            return Task.FromResult(PreconditionResult.FromError("No moderator roles configured."));
        }

        if (context.User is not SocketGuildUser user)
        {
            return Task.FromResult(PreconditionResult.FromError("User not recognized as guild user."));
        }

        if (user.Roles.Any(role => guild.ModeratorRoles.Contains(role.Id)))
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }

        return Task.FromResult(PreconditionResult.FromError("User has no recognized moderator role."));
    }
}