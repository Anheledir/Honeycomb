using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BaseBotService.Utilities.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAllImplementationsOf<TInterface>(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // Get all types that implement the specified interface
        var implementingTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface && typeof(TInterface).IsAssignableFrom(type));

        // Register each implementing type with the DI container
        foreach (var type in implementingTypes)
        {
            services.Add(new ServiceDescriptor(type, type, lifetime)); // Register as itself
            services.Add(new ServiceDescriptor(typeof(TInterface), type, lifetime)); // Register as TInterface
            Console.WriteLine($"Registered {type.Name} as {typeof(TInterface).Name} with lifetime {lifetime}.");
        }

        return services;
    }
}