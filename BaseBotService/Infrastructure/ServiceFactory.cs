﻿using BaseBotService.Commands;
using BaseBotService.Core;
using BaseBotService.Core.Interfaces;
using BaseBotService.Core.Messages;
using BaseBotService.Data;
using BaseBotService.Data.Models;
using BaseBotService.Infrastructure.Behaviors;
using BaseBotService.Infrastructure.Services;
using BaseBotService.Infrastructure.Strategies;
using BaseBotService.Interactions;
using BaseBotService.Utilities;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
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
        // log services
            .AddSingleton(LoggerFactory.CreateLogger())
            .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))

        // discord services
            .AddSingleton(socketConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(serviceConfig)
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))

        // events
            .AddSingleton<DiscordEventListener>()

        // strategies
            .AddSingleton<ActionRowStrategy>()
            .AddSingleton<ButtonStrategy>()
            .AddSingleton<ModalSubmitStrategy>()
            .AddSingleton<SelectMenuStrategy>()
            .AddSingleton<TextInputStrategy>()

        // misc services
            .AddSingleton<IAssemblyService, AssemblyService>()
            .AddSingleton<IEnvironmentService, EnvironmentService>()
            .AddScoped<IEngagementService, EngagementService>()

        // utilities
            .AddSingleton<RateLimiter>()

            // behaviors
            //.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))

            // persistence services
            .AddSingleton<IPersistenceService, PersistenceService>()
            .AddScoped(MemberHC.GetServiceRegistration)
            .AddScoped(GuildMemberHC.GetServiceRegistration);

        return services.BuildServiceProvider();
    }
}