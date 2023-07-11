using Discord.WebSocket;

namespace BaseBotService.Utilities.Attributes;

internal class DoUserCheckAttribute : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        // Check if the component matches the target properly.
        if (context.Interaction is not SocketMessageComponent componentContext)
        {
            return Task.FromResult(PreconditionResult.FromError("Context unrecognized as component context."));
        }

        // The approach here entirely depends on how you construct your custom ID. In this case, the format is:
        // unique-name:*,*

        // here the name and wildcards are split by ':'
        string[] param = componentContext.Data.CustomId.Split(':');

        // here we determine that we should always check for the first ',' present.
        // This will deal with additional wildcards by always selecting the first wildcard present.
        if (param.Length > 1 && ulong.TryParse(param[1].Split(',')[0], out ulong id))
        {
            return (context.User.Id == id)
                ? Task.FromResult(PreconditionResult.FromSuccess())
                : Task.FromResult(PreconditionResult.FromError("User ID does not match component ID!"));
        }

        return Task.FromResult(PreconditionResult.FromError("Parse cannot be done if no userID exists."));
    }
}