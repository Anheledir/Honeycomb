namespace BaseBotService.Commands.Modals;

public class PollsCreateOptionModal : IModal
{
    public required string Title { get; set; }

    [ModalTextInput("text")]
    public string? OptionName { get; set; }

    [ModalTextInput("emoji")]
    public string? OptionEmoji { get; set; }
}