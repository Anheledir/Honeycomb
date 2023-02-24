using BaseBotService.Base;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using BaseBotService.Modules;
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

            // base services
            services.AddSerilogServices(loggerConfig);
            services.AddSingleton<DiscordSocketClient, DiscordSocketClient>();
            services.AddSingleton<CommandService, CommandService>();
            services.AddSingleton<ICommandHandler, CommandHandler>();

            // misc services
            services.AddTransient<IAssemblyService, AssemblyService>();
            services.AddTransient<IEnvironmentHelper, EnvironmentHelper>();

            // module services
            services.AddTransient(typeof(InfoModule));
            services.AddTransient(typeof(UsersModule));

            return services.BuildServiceProvider();
        }
    }
}