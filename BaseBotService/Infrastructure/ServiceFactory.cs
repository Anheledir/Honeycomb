using BaseBotService.Commands;
using BaseBotService.Commands.Interfaces;
using BaseBotService.Core;
using BaseBotService.Core.Base;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Models;
using BaseBotService.Data.Repositories;
using BaseBotService.Infrastructure.Achievements;
using BaseBotService.Infrastructure.Services;
using BaseBotService.Utilities;
using BaseBotService.Utilities.Extensions;
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
            .AddSingleton<CancellationTokenSource>()

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
            .AddSingleton<IDateTimeProvider, NodaDateTimeService>()
            .AddSingleton<RateLimiter>()
            .AddScoped<IEngagementService, EngagementService>()
            .AddScoped<IEasterEventService, EasterEventService>()
            .AddScoped<IPermissionService, PermissionService>()

        // data services
            .AddSingleton<IPersistenceService, PersistenceService>()
            .AddSingleton<MigrationManager>()
            .AddSingleton<CollectionMapper>()

        // data repositories
            .AddScoped<IGuildRepository, GuildRepository>()
            .AddScoped<IGuildMemberRepository, GuildMemberRepository>()
            .AddScoped<IMemberRepository, MemberRepository>()
            .AddScoped(typeof(IAchievementRepository<>), typeof(AchievementRepository<>))

        // data achievement models
            .AddScoped(AchievementBase.GetServiceRegistration<AchievementBase>)
            .AddScoped<AchievementBase, EasterEventAchievement>()
            .AddScoped<EasterEventAchievement>()

        // data models
            .AddScoped(GuildHC.GetServiceRegistration<GuildHC>)
            .AddScoped(MemberHC.GetServiceRegistration<MemberHC>)
            .AddScoped(GuildMemberHC.GetServiceRegistration<GuildMemberHC>)

        // data migrations
            .AddAllImplementationsOf<IMigrationChangeset>(typeof(IMigrationChangeset).Assembly, ServiceLifetime.Scoped);

        return services.BuildServiceProvider();
    }
}