using BaseBotService.Events;
using BaseBotService.Interfaces;
using BaseBotService.Models;
using BaseBotService.Modules;
using BaseBotService.Services;
using BaseBotService.Utilities;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BaseBotService.Factories;

public static class ServiceFactory
{
    public static IServiceProvider CreateServiceProvider()
    {
        DiscordSocketConfig socketConfig = new()
        {
            LogGatewayIntentWarnings = false,
            GatewayIntents = GatewayIntents.AllUnprivileged,
            AlwaysDownloadUsers = true,
        };

        InteractionServiceConfig serviceConfig = new()
        {
            DefaultRunMode = RunMode.Async,
            InteractionCustomIdDelimiters = new[] { '.' },
            EnableAutocompleteHandlers = true,
            AutoServiceScopes = true
        };

        IServiceCollection services = new ServiceCollection()
        // log services
            .AddSingleton(LoggerFactory.CreateLogger())

        // discord services
            .AddSingleton(socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(serviceConfig)
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))

        // events
            .AddSingleton<DiscordEvents>()

        // misc services
            .AddSingleton<IAssemblyService, AssemblyService>()
            .AddSingleton<IEnvironmentService, EnvironmentService>()
            .AddScoped<IEngagementService, EngagementService>()

        // utilities
            .AddSingleton<RateLimiter>()

        //// command modules
        //    .AddSingleton<BotModule>()
        //    .AddSingleton<UserModule>()

        // persistence services
            .AddSingleton<IPersistenceService, PersistenceService>()
            .AddScoped(MemberHC.GetServiceRegistration)
            .AddScoped(GuildMemberHC.GetServiceRegistration);

        return services.BuildServiceProvider();
    }
}