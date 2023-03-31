using BaseBotService.Commands;
using BaseBotService.Commands.Interfaces;
using BaseBotService.Core;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Infrastructure.Services;
using BaseBotService.Utilities;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaseBotService.Infrastructure;

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
        // core services
            .AddSingleton(LoggerFactory.CreateLogger())
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))

            // discord services
            .AddSingleton(socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(serviceConfig)
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<DiscordEventListener>()

        // command modules
            .AddScoped<UserModule>()
            .AddScoped<BotModule>()

        // utilities
            .AddSingleton<ITranslationService>(_ => new TranslationService(TranslationFactory.CreateMessageContexts()))
            .AddSingleton<IAssemblyService, AssemblyService>()
            .AddSingleton<IEnvironmentService, EnvironmentService>()
            .AddScoped<IEngagementService, EngagementService>()
            .AddSingleton<RateLimiter>()

        // data services
            .AddSingleton<IPersistenceService, PersistenceService>()

        // data repositories
            .AddSingleton<IGuildMemberHCRepository, GuildMemberHCRepository>()
            .AddSingleton<IMemberHCRepository, MemberHCRepository>()

        // data models
            .AddScoped(MemberHC.GetServiceRegistration)
            .AddScoped(GuildMemberHC.GetServiceRegistration);

        return services.BuildServiceProvider();
    }
}