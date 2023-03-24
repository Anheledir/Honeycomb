using BaseBotService.Core.Messages;

namespace BaseBotService.Interactions;
internal class ModalSubmitHandler : IRequestHandler<ModalSubmitRequest>
{
    public Task Handle(ModalSubmitRequest request, CancellationToken cancellationToken) => throw new NotImplementedException();
}
