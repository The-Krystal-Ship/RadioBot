
using Microsoft.Extensions.DependencyInjection;

using RadioBot.Handlers;
using RadioBot.Services;

namespace RadioBot.Extensions
{
    public static class ServiceExtensions
    {
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
