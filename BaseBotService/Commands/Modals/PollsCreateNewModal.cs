namespace BaseBotService.Commands.Modals;

public class PollsCreateNewModal : IModal
{
    public required string Title { get; set; }

    [ModalTextInput("title")]
    public required string PollTitle { get; set; }

    [ModalTextInput("description")]
    public string? Description { get; set; }
}