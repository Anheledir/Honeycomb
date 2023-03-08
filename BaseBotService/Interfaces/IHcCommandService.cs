namespace BaseBotService.Interfaces;

public interface IHcCommandService
{
    Task RegisterGlobalCommandsAsync(bool overwrite = false);
}