using BaseBotService.Events;
using BaseBotService.Extensions;
using BaseBotService.Interfaces;
using BaseBotService.Models;
using BaseBotService.Modules;
using BaseBotService.Services;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Managers;

public static class ServiceManager
{

    public static IServiceProvider RegisterServices()
    {
        var config = new DiscordSocketConfig()
        {
            LogGatewayIntentWarnings = false
        };

        var servConfig = new InteractionServiceConfig()
        {
            DefaultRunMode = Discord.Interactions.RunMode.Async
        };

        var services = new ServiceCollection()

        // log service
            .AddSerilogServices()

        // discord services
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(servConfig)
            .AddSingleton<CommandService>()
            .AddSingleton<InteractionService>()

        // event services
            .AddSingleton<DiscordSocketClientEvents>()

        // misc services
            .AddSingleton<HealthCheckService>()
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