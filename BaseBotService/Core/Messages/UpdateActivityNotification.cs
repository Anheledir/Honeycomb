namespace BaseBotService.Core.Messages;
public class UpdateActivityNotification : INotification
{
    public string Description { get; set; } = string.Empty;
    public IEmote? Emote { get; set; }
    public ActivityType ActivityType { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Online;
}