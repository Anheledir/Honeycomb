using Serilog;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace BaseBotService.Services
{
    public class AssemblyService : IAssemblyService
    {
        [SuppressMessage("Minor Code Smell", "S1450:Private fields only used as local variables in methods should become local variables", Justification = "Might be used at a later time.")]
        private readonly ILogger _logger;
        private readonly Assembly _assembly;

        public AssemblyService(ILogger logger)
        {
            _logger = logger;
            _assembly = Assembly.GetExecutingAssembly();
            _logger.Debug($"Initialized {nameof(AssemblyService)}");
        }

        public string Name => _assembly.GetName().Name!;
        public string Version => _assembly.GetName().Version!.ToString();
    }
}
