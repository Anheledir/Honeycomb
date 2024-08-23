using BaseBotService.Commands;
using BaseBotService.Commands.Interfaces;
using BaseBotService.Core;
using BaseBotService.Core.Interfaces;
using BaseBotService.Data;
using BaseBotService.Data.Interfaces;
using BaseBotService.Data.Repositories;
using BaseBotService.Infrastructure.Services;
using BaseBotService.Utilities;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaseBotService.Infrastructure;

public static class ServiceFactory
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // Core services
        ConfigureCoreServices(services);
        ConfigureDiscordServices(services);
        ConfigureCommandModules(services);
        ConfigureUtilities(services);
        ConfigureRepositories(services);
    }

    private static void ConfigureCoreServices(IServiceCollection services)
    {
        services.AddSingleton(LoggerFactory.CreateLogger())
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddSingleton<CancellationTokenSource>()
                .AddSingleton<IEnvironmentService, EnvironmentService>()
                .AddSingleton<AchievementFactory>()
                .AddDbContext<HoneycombDbContext>((provider, options) =>
                {
                    var envService = provider.GetRequiredService<IEnvironmentService>();
                    options.UseSqlite(envService.ConnectionString);
                });
    }

    private static void ConfigureDiscordServices(IServiceCollection services)
    {
        DiscordSocketConfig socketConfig = new()
        {
            LogGatewayIntentWarnings = false,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
        };

        InteractionServiceConfig serviceConfig = new()
        {
            DefaultRunMode = RunMode.Async,
            InteractionCustomIdDelimiters = new[] { '.' },
            EnableAutocompleteHandlers = true,
            AutoServiceScopes = true
        };

        services.AddSingleton(socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(serviceConfig)
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<DiscordEventListener>();
    }

    private static void ConfigureCommandModules(IServiceCollection services)
    {
        services.AddScoped<UserModule>()
                .AddScoped<BotModule>();
    }

    private static void ConfigureUtilities(IServiceCollection services)
    {
        services.AddSingleton<ITranslationService>(_ => new TranslationService(TranslationFactory.CreateMessageContexts()))
                .AddSingleton<IAssemblyService, AssemblyService>()
                .AddSingleton<IDateTimeProvider, NodaDateTimeService>()
                .AddSingleton<RateLimiter>()
                .AddScoped<IEngagementService, EngagementService>()
                .AddScoped<IEasterEventService, EasterEventService>()
                .AddScoped<IPermissionService, PermissionService>();
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<IGuildRepository, GuildRepository>()
                .AddScoped<IMemberRepository, MemberRepository>()
                .AddScoped<IGuildMemberRepository, GuildMemberRepository>()
                .AddScoped(typeof(IAchievementRepository<>), typeof(AchievementRepository<>));
    }
}
