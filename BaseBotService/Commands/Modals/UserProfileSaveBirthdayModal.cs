namespace BaseBotService.Commands.Modals;
public class UserProfileSaveBirthdayModal : IModal
{
    public string Title => string.Empty;

    [ModalTextInput("day")]
    public string Day { get; set; } = null!;

    [ModalTextInput("month")]
    public string Month { get; set; } = null!;

    [ModalTextInput("year")]
    public string? Year { get; set; }

    public bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Day) || string.IsNullOrWhiteSpace(Month))
        {
            return false;
        }
        if (int.TryParse(Day, out int day) && int.TryParse(Month, out int month))
        {
            if (day > 31 || day < 1 || month > 12 || month < 1)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    public DateTime? GetBirthday()
    {
        if (!Validate())
        {
            return null;
        }

        int year = string.IsNullOrWhiteSpace(Year) || int.Parse(Year) < DateTime.Now.Year - 100 ? 1 : int.Parse(Year);
        int month = int.Parse(Month);
        int day = int.Parse(Day);

        return new DateTime(year, month, day);
    }
}
