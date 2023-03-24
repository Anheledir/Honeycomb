﻿using BaseBotService.Core.Messages;
using Discord.WebSocket;

namespace BaseBotService.Interactions;
public class InteractionHandler : INotificationHandler<InteractionCreatedNotification>
{
    private readonly DiscordSocketClient _client;
    private readonly IMediator _mediator;

    public InteractionHandler(DiscordSocketClient client, IMediator mediator)
    {
        _client = client;
        _mediator = mediator;
    }

    public async Task Handle(InteractionCreatedNotification arg, CancellationToken cancellationToken)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of InteractionModuleBase<T> modules.
            SocketInteractionContext context = new(_client, arg.Interaction);
            await context.Interaction.DeferAsync();

            switch (arg.Interaction.Type)
            {
                case InteractionType.MessageComponent:
                    await _mediator.Send(new MessageComponentRequest(context), cancellationToken);
                    break;
                case InteractionType.ModalSubmit:
                    await _mediator.Send(new ModalSubmitRequest(context), cancellationToken);
                    break;
                case InteractionType.ApplicationCommandAutocomplete:
                    await _mediator.Send(new AutocompleteCommandRequest(context), cancellationToken);
                    break;
                case InteractionType.ApplicationCommand:
                    await _mediator.Send(new ApplicationCommandRequest(context), cancellationToken);
                    break;
            }
        }
        catch
        {
            // If Slash Command execution fails it is most likely that the original interaction acknowledgment
            // will persist. It is a good idea to delete the original response, or at least let the user know
            // that something went wrong during the command execution.
            if (arg.Interaction.Type is InteractionType.ApplicationCommand)
            {
                _ = await arg.Interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}
