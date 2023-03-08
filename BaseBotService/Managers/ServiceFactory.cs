using BaseBotService.Events;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using BaseBotService.Models;
using BaseBotService.Modules;
using BaseBotService.Services;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Managers;

public static class ServiceFactory
{
    public static IServiceProvider CreateServiceProvider()
    {
        var config = new DiscordSocketConfig()
        {
            LogGatewayIntentWarnings = false
        };

        var servConfig = new InteractionServiceConfig()
        {
            DefaultRunMode = Discord.Interactions.RunMode.Async
        };

        var telemetryConfig = TelemetryConfiguration.CreateDefault();

        using var channel = new InMemoryChannel();

        var services = new ServiceCollection()

        // log service
            .AddSerilogServices()
            .Configure<TelemetryConfiguration>(config => config.TelemetryChannel = channel)

            //.AddSingleton<ITelemetryInitializer>(new MyTelemetryInitializer())

            // discord services
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(servConfig)
            .AddSingleton<CommandService>()
            .AddSingleton<InteractionService>()

        // event services
            .AddSingleton<DiscordSocketClientEvents>()

        // misc services
            .AddSingleton<ICommandManager, CommandManager>()
            .AddSingleton<IAssemblyService, AssemblyService>()
            .AddSingleton<IEnvironmentService, EnvironmentService>()
            .AddScoped<IActivityPointsService, ActivityPointsService>()

        // command modules
            .AddSingleton<InfoModule>()
            .AddSingleton<UsersModule>()

        // persistence services
            .AddSingleton<IPersistenceService, PersistenceService>()
            .AddScoped(MemberHC.GetServiceRegistration)
            .AddScoped(GuildMemberHC.GetServiceRegistration);

        return services.BuildServiceProvider();
    }
}