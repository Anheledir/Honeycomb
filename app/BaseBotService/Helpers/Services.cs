using BaseBotService.Events;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using BaseBotService.Modules;
using BaseBotService.Services;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BaseBotService.Helpers;

public static class Services
{

    public static IServiceProvider RegisterServices()
    {
        // Create our Serilog configuration
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console();

        var config = new DiscordSocketConfig()
        {
            LogGatewayIntentWarnings = false
        };

        var servConfig = new InteractionServiceConfig()
        {
            DefaultRunMode = Discord.Interactions.RunMode.Async
        };

        var services = new ServiceCollection()

        // logging
            .AddSerilogServices(loggerConfig)

        // discord services
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(servConfig)
            .AddSingleton<CommandService>()
            .AddSingleton<InteractionService>()

        // misc services
            .AddSingleton<DiscordSocketClientEvents>()
            .AddSingleton<ICommandManager, CommandManager>()
            .AddSingleton<IAssemblyService, AssemblyService>()
            .AddSingleton<IEnvironmentService, EnvironmentService>()

        // modules
            .AddSingleton<InfoModule>()
            .AddSingleton<UsersModule>();

        return services.BuildServiceProvider();
    }
}