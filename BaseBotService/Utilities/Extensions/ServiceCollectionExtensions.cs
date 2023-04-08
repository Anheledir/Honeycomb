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
            var serviceDescriptor = new ServiceDescriptor(typeof(TInterface), type, lifetime);
            services.Add(serviceDescriptor);
        }

        return services;
    }
}