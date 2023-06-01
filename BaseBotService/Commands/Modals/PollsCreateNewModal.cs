namespace BaseBotService.Commands.Modals;
public class PollsCreateNewModal : IModal
{
    [ModalTextInput("title")]
    public string Title { get; set; } = null!;

    [ModalTextInput("description")]
    public string? Description { get; set; }
}
