using BaseBotService.Events;
using BaseBotService.Interfaces;
using BaseBotService.Models;
using BaseBotService.Modules;
using BaseBotService.Services;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Factories;

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

        var services = new ServiceCollection()
        // log services
            .AddSingleton(LoggerFactory.CreateLogger())

            // discord services
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(servConfig)
            .AddSingleton<CommandService>()
            .AddSingleton<InteractionService>()

        // event services
            .AddSingleton<DiscordSocketClientEvents>()

        // misc services
            .AddSingleton<IHcCommandService, HcCommandService>()
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