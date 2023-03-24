using BaseBotService.Core.Interfaces;
using System.Reflection;

namespace BaseBotService.Infrastructure.Services;

public class AssemblyService : IAssemblyService
{
    private readonly Assembly _assembly;

    public AssemblyService(ILogger logger)
    {
        _assembly = Assembly.GetExecutingAssembly();
        logger.Debug($"Initialized {nameof(AssemblyService)}");
    }

    public string Name => _assembly.GetName().Name!;
    public string Version => _assembly.GetName().Version!.ToString();
}
