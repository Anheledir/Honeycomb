namespace BaseBotService.Interfaces;

public interface ICommandManager
{
    Task RegisterGlobalCommandsAsync(bool overwrite = false);
}