using Inquisition.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using RadioBot.Handlers;
using RadioBot.Services;

namespace RadioBot.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLogger(this IServiceCollection services)
        {
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            return services;
        }

        public static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            services.AddSingleton<EventHandler>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<RadioService>();

            return services;
        }
    }
}
