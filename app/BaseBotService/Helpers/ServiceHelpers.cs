using BaseBotService.Base;
using BaseBotService.Extensions;
using BaseBotService.Services;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BaseBotService.Helpers
{
    public static class ServiceHelpers
    {

        public static IServiceProvider RegisterServices()
        {
            // Create our Serilog configuration
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            var services = new ServiceCollection();

            services.AddSerilogServices(loggerConfig);
            services.AddSingleton<DiscordSocketClient, DiscordSocketClient>();
            services.AddSingleton<CommandService, CommandService>();
            services.AddSingleton<ICommandHandler, CommandHandler>();
            services.AddTransient<IAssemblyService, AssemblyService>();

            return services.BuildServiceProvider();
        }
    }
}